﻿import * as React from "react";
import { Route, Switch } from "react-router";
import styled from "styled-components";
import Backdrop from "./navigation/Backdrop/Backdrop";
import Toolbar from "./navigation/Toolbar/Toolbar";
import SideDrawer from "./navigation/SideDrawer/SideDrawer";

export interface IAppProps {
  compiler: string;
  framework: string;
}
interface IAppState {
  sideDrawerOpen: boolean;
}

// export const Hello = (props: HelloProps) => <h1>Hello from {props.compiler} and {props.framework}!</h1>;

const Title = styled.h1`
  font-size: 1.5em;
  text-align: center;
  color: palevioletred;
`;

const HomePage = () => <div>Home Page</div>;
const UsersPage = () => <div>Users Page</div>;

export class App extends React.Component<IAppProps, IAppState> {
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
      <div style={{ height: "100%" }}>
        <Toolbar drawerClickHandler={this.drawerToggleClickHandler} />
        <SideDrawer show={this.state.sideDrawerOpen} />
        {backdrop}
        <main style={{ marginTop: "64px" }}>
          <Title>Styled Component</Title>
          <p>This is the page content!</p>
          {/* use switch for exclusive routing - just 1 match */}
          <Switch>
            <Route path="/" exact component={HomePage} />
            <Route path="/users" component={UsersPage} />
          </Switch>
        </main>
      </div>
    );
  }
}