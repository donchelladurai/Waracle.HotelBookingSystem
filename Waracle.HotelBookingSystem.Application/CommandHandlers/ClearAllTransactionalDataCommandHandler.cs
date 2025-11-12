using MediatR;
using Microsoft.Extensions.Logging;
using Waracle.HotelBookingSystem.Application.Commands;
using Waracle.HotelBookingSystem.Data.Repositories.Interfaces;

namespace Waracle.HotelBookingSystem.Application.CommandHandlers
{
    public class ClearAllTransactionalDataCommandHandler : IRequestHandler<ClearAllTransactionalDataCommand>
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IRoomsRepository _roomsRepository;
        private readonly ILogger<ClearAllTransactionalDataCommandHandler> _logger;

        public ClearAllTransactionalDataCommandHandler(
            IHotelsRepository hotelsRepository,
            IBookingsRepository bookingsRepository,
            IRoomsRepository roomsRepository,
            ILogger<ClearAllTransactionalDataCommandHandler> logger)
        {
            _hotelsRepository = hotelsRepository;
            _bookingsRepository = bookingsRepository;
            _roomsRepository = roomsRepository;
            _logger = logger;
        }

        public async Task Handle(ClearAllTransactionalDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //await _bookingsRepository.RemoveAllAsync(cancellationToken).ConfigureAwait(false);
                //await _roomsRepository.RemoveAllAsync(cancellationToken).ConfigureAwait(false);
                await _hotelsRepository.RemoveAllAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was canceled while seeding data");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while clearing transactional data");

                throw;
            }
        }
    }
}
