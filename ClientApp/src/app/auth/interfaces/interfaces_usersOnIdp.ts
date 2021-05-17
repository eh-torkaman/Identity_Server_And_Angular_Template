
export interface dbClaim {
    id: number;
    userId: string;
    claimType: string;
    claimValue: string;
}

export interface dbUser {
  userName: string;
  lockoutEnd: string;// Date?;
  lockoutEnabled: boolean;
  accessFailedCount: number;
  dbClaims: dbClaim[];
  profileImageNumber: number;
  isLockedOut: boolean;

  lockDescription: string;
  
}


export interface UserNameAndPassword {
  userName: string;
  password: string;
}

export interface ChangeUserPass {
  oldPassword: string;
  newPassword: string;
}

