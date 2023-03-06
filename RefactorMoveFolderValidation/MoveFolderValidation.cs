namespace RefactorMoveFolderValidation {
    public class MoveFolderValidation
    {
        private readonly User _currentUser;
        private readonly ICustomReportFolderService _customReportFolderService;

        public MoveFolderValidation( User currentUser, ICustomReportFolderService customReportFolderService)
        {
            this._currentUser = currentUser;
            this._customReportFolderService = customReportFolderService;
        }

        /// <summary>
        /// Validates whether the user is permitted to move the item into the target folder.
        /// </summary>
        public Result ValidateMoveFolderItem(IFolderItem itemToBeMoved, long? targetFolderId)
        {

            var result = new Result();

            IFolderItem targetFolder = targetFolderId.HasValue ? _customReportFolderService.Get(targetFolderId.Value) : null;

            bool targetFolderIsShared = targetFolder == null || targetFolder.IsShared;
            bool itemToBeMovedIsShared = itemToBeMoved.IsShared;
            bool itemToBeMovedIsOwnedByUser = itemToBeMoved.IsOwner(_currentUser);
            bool targetFolderIsOwnedByUser = targetFolder.IsOwner(_currentUser);


            // if the item is shared but the folder isn't then leave
            // we don't care about the owner as this applies first
            if (!itemToBeMovedIsShared && targetFolderIsShared)
            {
                result.FolderItemStatusCode = FolderItemStatusCode.SharedItemMayOnlyMoveToSharedFolder;
            }

            // user owns item, and target and both item and folder are shared
            // in this case we dont care about the owner of the shared target 
            else if( itemToBeMovedIsOwnedByUser && itemToBeMovedIsShared
                     && targetFolderIsShared )
            {
                result.FolderItemStatusCode = FolderItemStatusCode.FileMovementAllowed;
            }

            // user owns item and and neither item nor folder are shared
            else if(itemToBeMovedIsOwnedByUser && targetFolderIsOwnedByUser
                    && !itemToBeMovedIsShared && !targetFolderIsShared)
            {
                result.FolderItemStatusCode = FolderItemStatusCode.FileMovementAllowed;
            }

            // user owns item and folder and item is shared but target isn't 
            else if (itemToBeMovedIsOwnedByUser && targetFolderIsOwnedByUser
                     && itemToBeMovedIsShared && !targetFolderIsShared)
            {
                // this one is "fuzzy" does the item stay shared if it moves to an unshared folder
                // might contradict rule 1
                // we could simplify this with teh above rule
                // user owns item and folder and target isn't shared. 
                //TODO: ask about this case and swap to failure if needed . 
                result.FolderItemStatusCode = FolderItemStatusCode.FileMovementAllowed;
            }

            // user is an admin, they can move any shared file to a shared location regardless of
            // who owns the file or folder thus no need to check item user 
            else if(_currentUser.RoleId == RoleId.Admin && itemToBeMovedIsShared && targetFolderIsShared)
            {
                result.FolderItemStatusCode = FolderItemStatusCode.FileMovementAllowed;
            }
            // I believe all other scenarios are owner/access ones. 
            // TODO: Decide if this should be a generic message in order to capture any items
            //         that fall through 
            else
            {
                result.FolderItemStatusCode = FolderItemStatusCode.UserNotOwnerOrAdmin;

            }

            return result;
        }

    }
}
