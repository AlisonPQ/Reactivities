using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccesor _photoAccesor;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IPhotoAccesor photoAccesor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _photoAccesor = photoAccesor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.Include(person => person.Photos)
                                .FirstOrDefaultAsync(person => person.UserName == _userAccessor.GetUsername());

                if (user == null)
                    return null;

                var photo = user.Photos.FirstOrDefault(photo => photo.Id == request.Id);

                if (photo == null)
                    return null;

                if (photo.IsMain)
                    return Result<Unit>.Failure("You cannot delete your main photo");

                var resultDeletingPhotoCloudinary = await _photoAccesor.DeletePhoto(photo.Id);

                if (resultDeletingPhotoCloudinary == null)
                    return Result<Unit>.Failure("Problem deleting photo from Cloudinary");

                user.Photos.Remove(photo);

                var resultDeletingPhotoDB = await _context.SaveChangesAsync() > 0;

                if (resultDeletingPhotoDB)
                    return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Problem deleting photo from API");
            }
        }
    }
}
