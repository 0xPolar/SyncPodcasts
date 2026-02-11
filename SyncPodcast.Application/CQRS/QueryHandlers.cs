using MediatR;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Application.CQRS
{
    public class GetLibraryQueryHandler : IRequestHandler<GetLibraryQuery, List<LibraryDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        public GetLibraryQueryHandler(IUserRepository userRepository, ISubscriptionRepository subscriptionRepository)
        {
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
        }
        public async Task<List<LibraryDTO>> Handle(GetLibraryQuery request, CancellationToken ct)
        {
            User? user = await _userRepository.GetByIdAsync(request.UserId, ct);
            if (user == null)
            {
                throw new DomainException($"User with ID {request.UserId} not found.");
            }

            var library = await _subscriptionRepository.GetUserPodcastsAsync(request.UserId, ct);
            if (library == null)
            {
                throw new DomainException($"No podcasts found for user with ID {request.UserId}.");
            }

            List<LibraryDTO> libraryDTOs = new List<LibraryDTO>();
            foreach (Podcast? podcast in library)
            {
                libraryDTOs.Add(new LibraryDTO
                (
                    podcast.ID,
                    podcast.Title,
                    podcast.Author,
                    podcast.FeedUrl,
                    podcast.ArtworkUrl
                ));
            }

            return libraryDTOs;
        }
    }
}
