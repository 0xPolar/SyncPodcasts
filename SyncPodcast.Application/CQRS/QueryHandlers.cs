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

    public class SearchPodcastQueryHandler : IRequestHandler<SearchPodcastQuery, List<PodcastSearchResultDTO>>
    {
        private readonly IPodcastSearchService _podcastSearchService;
        public SearchPodcastQueryHandler(IPodcastSearchService podcastSearchService)
        {
            _podcastSearchService = podcastSearchService;
        }
        public async Task<List<PodcastSearchResultDTO>> Handle(SearchPodcastQuery request, CancellationToken ct)
        {
            var results = await _podcastSearchService.SearchAsync(request.Query, ct);
            List<PodcastSearchResultDTO> resultDTOs = new List<PodcastSearchResultDTO>();
            foreach (var result in results)
            {
                resultDTOs.Add(new PodcastSearchResultDTO
                (
                    result.ID,
                    result.Title,
                    result.Author,
                    result.FeedUrl,
                    result.ArtworkUrl
                ));
            }
            return resultDTOs;
        }
    }

    public class GetPodcastDetailsQueryHandler : IRequestHandler<GetPodcastDetailsQuery, PodcastDetailsDTO>
    {
        private readonly IPodcastRepository _podcastRepository;
        public GetPodcastDetailsQueryHandler(IPodcastRepository podcastRepository)
        {
            _podcastRepository = podcastRepository;
        }
        public async Task<PodcastDetailsDTO> Handle(GetPodcastDetailsQuery request, CancellationToken ct)
        {
            Podcast? podcast = await _podcastRepository.GetPodcastByIdAsync(request.PodcastId, ct);
            if (podcast == null)
            {
                throw new DomainException($"Podcast with ID {request.PodcastId} not found.");
            }
            List<EpisodeDTO> episodeDTOs = new List<EpisodeDTO>();
            foreach (var episode in podcast.Episodes)
            {
                episodeDTOs.Add(new EpisodeDTO
                (
                    episode.ID,
                    episode.Title,
                    episode.Duration,
                    episode.PublishedAt,
                    episode.MediaUrl
                ));
            }
            return new PodcastDetailsDTO
            (
                podcast.ID,
                podcast.Title,
                podcast.Author,
                podcast.FeedUrl,
                podcast.ArtworkUrl,
                episodeDTOs
            );
        }
    }

    public class GetPlaybackProgressQueryHandler : IRequestHandler<GetPlaybackProgressQuery, PlaybackProgressDTO>
    {
        private readonly IPlaybackProgressRepository _playbackProgressRepository;
        public GetPlaybackProgressQueryHandler(IPlaybackProgressRepository playbackProgressRepository)
        {
            _playbackProgressRepository = playbackProgressRepository;
        }
        public async Task<PlaybackProgressDTO> Handle(GetPlaybackProgressQuery request, CancellationToken ct)
        {
            PlaybackProgress? progress = await _playbackProgressRepository.GetAsync(request.UserId, request.EpisodeId, ct);
            if (progress == null)
            {
                return new PlaybackProgressDTO(request.EpisodeId, TimeSpan.Zero);
            }
            return new PlaybackProgressDTO(progress.EpisodeID, progress.Position);
        }
    }
}
