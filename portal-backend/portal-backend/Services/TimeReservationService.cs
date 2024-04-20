using Microsoft.EntityFrameworkCore;
using portal_backend.Entities;
using portal_backend.Enums;
using portal_backend.Models;

namespace portal_backend.Services;

public class TimeReservationService
{
    private readonly VcvsContext _vcvsContext;
    
    public TimeReservationService(VcvsContext vcvsContext)
    {
        _vcvsContext = vcvsContext;
    }
    
    public void ReserveTimes(List<TimeReservationModel> reservations, Service service)
    {
        foreach (var reservation in reservations)
        {
            ReserveTime(reservation, service);
        }
    }
    
    public async Task RemoveReservation(int reservationId, int userId)
    {
        var reservation = _vcvsContext.FullOrder
            .Include(x => x.Order)
            .FirstOrDefault(x => x.Id == reservationId && x.Service.Specialist.User.Id == userId);

        if (reservation is null)
        {
            throw new Exception("Reservation doesn't exist");
        }

        if (reservation.DateFrom < DateTime.Now)
        {
            throw new Exception("Reservation happened in the past");
        }

        var order = reservation.Order;

        if (order is not null)
        {
            order.OrderStatus = OrderStatus.Cancelled;
        }

        _vcvsContext.FullOrder.Remove(reservation);
    }
    
    public async Task UpdateReservation(TimeReservationModel timeReservationModel, int userId)
    {
        var reservation = _vcvsContext.FullOrder
            .Include(x => x.Equipment)
            .Include(x => x.Order)
            .FirstOrDefault(x =>
            x.Id == timeReservationModel.Id && x.Service.Specialist.User.Id == userId);

        if (reservation is null)
        {
            throw new Exception("Reservation doesn't exist");
        }

        if (reservation.DateFrom < DateTime.Now)
        {
            throw new Exception("Reservation happened in the past");
        }
        
        var order = reservation.Order;

        if (order is not null)
        {
            order.OrderStatus = OrderStatus.Cancelled;
            reservation.Order = null;
        }
        
        var room = _vcvsContext.Room.FirstOrDefault(x => x.Id == timeReservationModel.RoomId);
        var givenEquipment = _vcvsContext.Equipment.Where(x => (timeReservationModel.EquipmentIds ?? new List<int>()).Contains(x.Id)).ToList();
        
        var equipmentToDelete = (reservation.Equipment ?? new List<Equipment>())
            .Where(x => !givenEquipment
                .Select(y => y.Id)
                .Contains(x.Id))
            .ToList();
        
        var equipmentToAdd = givenEquipment
            .Where(x => !(reservation.Equipment ?? new List<Equipment>())
                .Select(y => y.Id)
                .Contains(x.Id))
            .ToList();
        
        foreach (var equipment in equipmentToDelete)
        {
            reservation.Equipment!.Remove(equipment);
        }

        foreach (var equipment in equipmentToAdd)
        {
            reservation.Equipment!.Add(equipment);
        }
        
        reservation.DateFrom = timeReservationModel.DateFrom;
        reservation.DateTo = timeReservationModel.DateTo;
        reservation.Room = room;
    }

    public void ValidateLocalReservations(List<TimeReservationModel> reservations)
    {
        for (var i = 0; i < reservations.Count - 1; i++)
        {
            for (var j = i + 1; j < reservations.Count; j++)
            {
                if (!(reservations[j].DateTo <= reservations[i].DateFrom || reservations[i].DateTo <= reservations[j].DateFrom))
                {
                    throw new Exception("Provided reservations happens at the same time");
                }
            }
        }
    }
    
    public void ValidateReservation(TimeReservationModel reservation, int specialistId, List<TimeReservationModel>? toDeleteList = null)
    {
        if (reservation.DateFrom >= reservation.DateTo)
        {
            throw new Exception("TimeReservation model has DateFrom >= DateTo");
        }

        if (reservation.DateFrom < DateTime.Now)
        {
            throw new Exception("Reservation cannot happen in the past");
        }

        var deleted = new List<TimeReservationModel>();

        if (toDeleteList is not null)
        {
            deleted = new List<TimeReservationModel>(toDeleteList);
        }

        var occupation =
            _vcvsContext.FullOrder
                .Where(x => !deleted
                    .Select(y => y.Id)
                    .Contains(x.Id))
                .FirstOrDefault(x =>
                x.Service.Specialist.Id == specialistId
                && !(reservation.DateTo <= x.DateFrom || x.DateTo <= reservation.DateFrom));
        
        if (occupation is not null)
        {
            throw new Exception("Time is already in use");
        }
    }

    public void ValidateBusinessCenterReservation(
        TimeReservationModel reservation,
        int specialistId,
        List<TimeReservationModel>? toDelete = null)
    {
        ValidateReservation(reservation, specialistId, toDelete);
        ValidateRoomReservation(reservation.RoomId ?? -1, reservation.DateFrom, reservation.DateTo, toDelete);

        reservation.EquipmentIds!.ForEach(x =>
            ValidateEquipmentReservation(x, reservation.DateFrom, reservation.DateTo, toDelete));
    }

    private void ValidateRoomReservation(int roomId, DateTime dateFrom, DateTime dateTo, List<TimeReservationModel>? toDeleteList = null)
    {
        var room = _vcvsContext.Room.FirstOrDefault(x => x.Id == roomId);

        if (room is null)
        {
            throw new Exception("Room doesn't exist");
        }

        if (!room.IsAvailable)
        {
            throw new Exception("Room is not available");
        }
        
        var deleted = new List<TimeReservationModel>();

        if (toDeleteList is not null)
        {
            deleted = new List<TimeReservationModel>(toDeleteList);
        }

        var occupation = _vcvsContext.FullOrder
            .Where(x => !deleted
                .Select(y => y.Id)
                .Contains(x.Id))
            .Where(x => x.Room!.Id == room.Id)
            .FirstOrDefault(y =>
                !(dateTo <= y.DateFrom|| y.DateTo <= dateFrom));

        if (occupation is not null)
        {
            throw new Exception("Room is already in use");
        }
    }
    
    private void ValidateEquipmentReservation(int equipmentId, DateTime dateFrom, DateTime dateTo, List<TimeReservationModel>? toDeleteList = null)
    {
        var equipment = _vcvsContext.Equipment.FirstOrDefault(x => x.Id == equipmentId);

        if (equipment is null)
        {
            throw new Exception("Equipment doesn't exist");
        }
        
        if (!equipment.IsAvailable)
        {
            throw new Exception("Equipment is not available");
        }
        
        var deleted = new List<TimeReservationModel>();

        if (toDeleteList is not null)
        {
            deleted = new List<TimeReservationModel>(toDeleteList);
        }

        var occupation = _vcvsContext.FullOrder
            .Where(x => !deleted
                .Select(y => y.Id)
                .Contains(x.Id))
            .Where(x => x.Equipment!.Select(y => y.Id).Contains(equipment.Id))
            .FirstOrDefault(y =>
                !(dateTo <= y.DateFrom|| y.DateTo <= dateFrom));
        
        
        if (occupation is not null)
        {
            throw new Exception("Equipment is already in use");
        }
    }
    
    private void ReserveTime(TimeReservationModel reservation, Service service)
    {

        var room = _vcvsContext.Room.FirstOrDefault(x => x.Id == reservation.RoomId);
        var equipment = _vcvsContext.Equipment.Where(x => (reservation.EquipmentIds ?? new List<int>()).Contains(x.Id)).ToList();

        var fullOrder = new FullOrder()
        {
            DateFrom = reservation.DateFrom,
            DateTo = reservation.DateTo,
            Room = room,
            Service = service,
            Equipment = equipment
        };

        _vcvsContext.FullOrder.Add(fullOrder);
    }
}