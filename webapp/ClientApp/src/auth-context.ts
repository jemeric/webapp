import * as React from "react";

export enum UserRole {
    ADMIN,
    USER,
    GUEST
}

export interface IUserContext {
  id: string; // data.sub
  email: string | null;
  role: "Admin" | "User" | "Guest";
}

export interface IAuthContextData {
  authenticated: boolean; // to check if authenticated or not
  user: IUserContext; // store all the user details
}

export class AuthContext implements IAuthContextData {
  public authenticated: boolean;
  public user: IUserContext;
  constructor(contextData: IAuthContextData) {
    this.authenticated = contextData.authenticated;
    this.user = contextData.user;
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