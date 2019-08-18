import * as React from "react";
import { NavItem } from "./NavItem";

interface IToolbarProps {
  drawerClickHandler: () => void;
}

interface INavMenuState {
  sideDrawerOpen: boolean;
  selectedNavId: string | null;
}

export class NavMenu extends React.Component<IToolbarProps, INavMenuState> {
  constructor(props: any) {
    super(props);
    this.selectNavItem = this.selectNavItem.bind(this);
  }

  public state: INavMenuState = {
    sideDrawerOpen: false,
    selectedNavId: null
  };

  private selectNavItem = (navId: string) => {
    this.setState({ selectedNavId: navId });
  };

  public render() {
    const showStyle = { display: "none" };
    return (
      <nav>
        <ul className="nav-list">
          <NavItem
            navId="home"
            navTitle="Home"
            selectedNavId={this.state.selectedNavId}
            selectNavItem={this.selectNavItem}
          >
            <a href="#!">Home</a>
          </NavItem>
          <NavItem
            navId="about"
            navTitle="About"
            selectedNavId={this.state.selectedNavId}
            selectNavItem={this.selectNavItem}
          >
            <a href="#!">About</a>
          </NavItem>
          <NavItem
            navId="services"
            navTitle="Services"
            selectedNavId={this.state.selectedNavId}
            selectNavItem={this.selectNavItem}
          >
            <a href="#!">Web Design</a>
            <a href="#!">Web Developer</a>
          </NavItem>
          <NavItem
            navId="portfolio"
            navTitle="Portfolio"
            selectedNavId={this.state.selectedNavId}
            selectNavItem={this.selectNavItem}
          >
            <a href="#!">Web Design</a>
            <a href="#!">Web Developer</a>
          </NavItem>
        </ul>
      </nav>
    );
  }
}
