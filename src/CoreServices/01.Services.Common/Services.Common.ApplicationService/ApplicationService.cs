using AutoMapper;
using Services.Common.Authorization;
using Services.Common.Domain.Uow;
using Services.Common.RunTime;

namespace Services.Common.ApplicationService
{
    /// <summary>
    /// Application Services are used to expose domain logic to the presentation layer.
    /// An Application Service is called from the presentation layer using a DTO (Data Transfer Object) as a parameter.
    /// It also uses domain objects to perform some specific business logic and returns a DTO back to the presentation layer.
    /// Thus, the presentation layer is completely isolated from Domain layer.
    /// </summary>
    public abstract class ApplicationService
    {
        protected ApplicationService(IUnitOfWork unitOfWork, IMapper mapper, IPermissionChecker permissionChecker, IUserSessionInfo userSessionInfo)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            PermissionChecker = permissionChecker;
            UserSessionInfo = userSessionInfo;
        }

        /// <summary>
        /// Reference to the Unit Of Work
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Reference to the AutoMapper.
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Reference to the permission checker.
        /// </summary>
        protected IPermissionChecker PermissionChecker { get; }

        /// <summary>
        /// Gets current session information.
        /// </summary>
        protected IUserSessionInfo UserSessionInfo { get; }
    }
}