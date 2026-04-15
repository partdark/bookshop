using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewsRepository _reviewsRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly ICustomersRepository _customersRepository;
        private readonly IRatingService _ratingService;

        public ReviewService(IReviewsRepository reviewsRepository, IBooksRepository booksRepository,
            ICustomersRepository customersRepository, IRatingService ratingService)
        {
            _reviewsRepository = reviewsRepository;
            _booksRepository = booksRepository;
            _customersRepository = customersRepository;
            _ratingService = ratingService;
        }

        public async Task<Guid> Add(AddReviewDto reviewDto)
        {
            var book = await _booksRepository.GetByIdAsync(reviewDto.BookId);
            if (book == null)
                throw new ArgumentException($"Book with ID {reviewDto.BookId} not found.");

            var customer = await _customersRepository.GetByIdAsync(reviewDto.CustomerId);
            if (customer == null)
                throw new ArgumentException($"Customer with ID {reviewDto.CustomerId} not found.");

           
            var existing = await _reviewsRepository.GetByCustomerAndBookAsync(reviewDto.CustomerId, reviewDto.BookId);
            if (existing != null)
                throw new ArgumentException("Вы уже оставляли отзыв на эту книгу.");

            var review = new Review
            {
                Rating = reviewDto.Rating,
                ReviewText = reviewDto.ReviewText,
                BookId = reviewDto.BookId,
                CustomerId = reviewDto.CustomerId,
            };
            await _reviewsRepository.AddAsync(review);
            await _ratingService.RecalculateAsync(review.BookId);
            return review.Id;
        }

        public async Task<bool> Delete(Guid id)
        {
            var review = await _reviewsRepository.GetByIdAsync(id);
            var bookId = review?.BookId;
            var result = await _reviewsRepository.DeleteAsync(id);
            if (result && bookId.HasValue)
                await _ratingService.RecalculateAsync(bookId.Value);
            return result;
        }

        public async Task<List<ReviewResponseDto>> GetAll()
        {
            var reviews = await _reviewsRepository.GetAll();
            return reviews.Select(r => new ReviewResponseDto(
                r.Id,
                r.Date,
                r.Rating,
                r.ReviewText,
                new CustomerResponseIdNameDto(r.Customer.Id, r.Customer.UserName)
            )).ToList();
        }

        public async Task<ReviewResponseDto?> GetById(Guid id)
        {
            var review = await _reviewsRepository.GetByIdAsync(id);
            if (review == null)
            {
                return null;
            }
            return new ReviewResponseDto(
                review.Id,
                review.Date,
                review.Rating,
                review.ReviewText,
                new CustomerResponseIdNameDto(review.Customer.Id, review.Customer.UserName)
            );
        }

        public async Task<ReviewResponseDto?> Update(UpdateReviewDto reviewDto)
        {
            var existingReview = await _reviewsRepository.GetByIdAsync(reviewDto.Id);
            if (existingReview == null)
            {
                return null;
            }

            existingReview.Rating = reviewDto.Rating;
            existingReview.ReviewText = reviewDto.ReviewText;

            var updatedReview = await _reviewsRepository.UpdateAsync(existingReview);
            if (updatedReview == null) return null;

            await _ratingService.RecalculateAsync(updatedReview.BookId);
            return new ReviewResponseDto(
                updatedReview.Id,
                updatedReview.Date,
                updatedReview.Rating,
                updatedReview.ReviewText,
                new CustomerResponseIdNameDto(updatedReview.Customer.Id, updatedReview.Customer.UserName)
            );
        }
    }
}
