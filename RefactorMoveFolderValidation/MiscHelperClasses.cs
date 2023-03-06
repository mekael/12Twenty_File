namespace RefactorMoveFolderValidation {

    public interface IFolderItem {
        bool IsShared { get; set; }
        bool IsOwner(User user);
    }

    public class User {
        public long Id { get; set; }
        public RoleId RoleId { get; set; }
    }

    public interface ICustomReportFolderService {
        IFolderItem Get(long id);
    }


    public enum FolderItemStatusCode
    {
        SharedItemMayOnlyMoveToSharedFolder,
        UserNotOwnerOrAdmin,
        FileMovementAllowed,
        FileMovementNotAllowed
    }

    // not the prettiest way to do this, but it works if we just want
    // to get it out of the way.
    // utilizing enum means we don't have to worry about string compare later
    // and it's more descriptive
    public class Result {

        // default failure during init compared to default to pass
        // this also takes care of someone rearranging the order of the enum (enums default to position 0) 
        public FolderItemStatusCode FolderItemStatusCode { get; set; } = FolderItemStatusCode.FileMovementNotAllowed;

        public bool IsSuccessful { get { return FolderItemStatusCode == FolderItemStatusCode.FileMovementAllowed; } }  
    }

    public enum RoleId {
        Admin = 1,
        Student = 2,
    }
}
