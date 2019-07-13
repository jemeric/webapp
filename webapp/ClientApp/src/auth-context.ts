import * as React from "react";

export enum UserRole {
    ADMIN,
    USER,
    GUEST
}

export interface IUserContext {
  id: string; // data.sub
  email: string | null;
  role: UserRole;
}

export interface IAuthContextData {
  authenticated: boolean; // to check if authenticated or not
  user: IUserContext; // store all the user details
  accessToken: string | null; // accessToken of user for authorization (To-Do - change with cookie auth?)
}

export class AuthContext implements IAuthContextData {
  public authenticated: boolean;
  public user: IUserContext;
  public accessToken: string | null;
  constructor(contextData: IAuthContextData) {
    this.authenticated = contextData.authenticated;
    this.user = contextData.user;
    this.accessToken = contextData.accessToken;
  }
  public initiateLogin() {
    // start the login process
  }
  public handleAuthentication() {
    // handle login process
  }
  public logout() {
    // logout the user
  }
}

export const {Provider, Consumer} = React.createContext<AuthContext | null>(null);