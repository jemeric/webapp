﻿import * as React from "react";
import { Route, Switch } from "react-router";
import styled from "styled-components";
import Backdrop from "./navigation/Backdrop/Backdrop";
import Toolbar from "./navigation/Toolbar/Toolbar";
import SideDrawer from "./navigation/SideDrawer/SideDrawer";
import { Consumer } from "../auth-context";
import { Layout } from "./navigation2/Layout";

export interface IAppProps {
  compiler: string;
  framework: string;
}
interface IAppState {
  sideDrawerOpen: boolean;
}

// export const Hello = (props: HelloProps) => <h1>Hello from {props.compiler} and {props.framework}!</h1>;

const Title = styled.h1<any>`
  font-size: 1.5em;
  text-align: center;
  margin-bottom: 1000px;
  color: palevioletred;
`;

const Paragraph = styled.p<any>`
  text-align: right;
`;

const HomePage = () => <div>Home Page</div>;
const UsersPage = () => <div>Users Page</div>;

export class App extends React.Component<IAppProps, IAppState> {
  constructor(props: any) {
    super(props);
  }

  public state: IAppState = {
    sideDrawerOpen: false
  };

  private drawerToggleClickHandler = () => {
    this.setState(prevState => {
      return { sideDrawerOpen: !prevState.sideDrawerOpen };
    });
  };

  private backdropClickHandler = () => {
    this.setState({ sideDrawerOpen: false });
  };

  public render() {
    let backdrop;

    if (this.state.sideDrawerOpen) {
      backdrop = <Backdrop click={this.backdropClickHandler} />;
    }
    return (
      <div>
        <Layout drawerClickHandler={this.drawerToggleClickHandler}>
          <Title>Styled Component</Title>
          <Paragraph>This is the page content!</Paragraph>

          <Consumer>
            {auth =>
              auth &&
              (auth.authenticated ? (
                <form method="POST" action="logout">
                  <button
                    className="btn btn-primary btn-login"
                    style={{ margin: "10px" }}
                  >
                    Logout {auth.user.role}
                  </button>
                </form>
              ) : (
                <div>Not Authenticated {auth.user.role}</div>
              ))
            }
          </Consumer>

          {/* use switch for exclusive routing - just 1 match */}
          <Switch>
            <Route path="/" exact component={HomePage} />
            <Route path="/users" component={UsersPage} />
          </Switch>
        </Layout>
      </div>
    );
  }
}
