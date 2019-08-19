import * as React from "react";
import { NavItem } from "./NavItem";
import { NavLink } from "react-router-dom";

interface INavMenuProps {
  shouldCloseNavItem: (event: MouseEvent) => boolean;
}

interface INavMenuState {
  sideDrawerOpen: boolean;
  selectedNavId: string | null;
}

export class NavMenu extends React.Component<INavMenuProps, INavMenuState> {
  private wrapperRef: HTMLElement | null = null;
  constructor(props: any) {
    super(props);
    this.selectNavItem = this.selectNavItem.bind(this);
    this.setWrapperRef = this.setWrapperRef.bind(this);
    this.handleClickOutside = this.handleClickOutside.bind(this);
  }

  public componentDidMount() {
    document.addEventListener("mousedown", this.handleClickOutside);
  }

  public componentWillUnmount() {
    document.removeEventListener("mousedown", this.handleClickOutside);
  }

  private setWrapperRef = (node: HTMLElement) => {
    this.wrapperRef = node;
  };

  private handleClickOutside = (event: MouseEvent) => {
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    if (
      this.wrapperRef &&
      !this.wrapperRef.contains(event.target as any) &&
      this.props.shouldCloseNavItem(event)
    ) {
      this.setState({ selectedNavId: null });
    }
  };

  public state: INavMenuState = {
    sideDrawerOpen: false,
    selectedNavId: null
  };

  private selectNavItem = (navId: string) => {
    if (this.state.selectedNavId === navId) {
      // unset the navigation if null
      this.setState({ selectedNavId: null });
    } else {
      this.setState({ selectedNavId: navId });
    }
  };

  public render() {
    const showStyle = { display: "none" };
    return (
      <nav ref={this.setWrapperRef}>
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
