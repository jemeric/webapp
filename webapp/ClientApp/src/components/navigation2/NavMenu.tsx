import * as React from "react";
import { NavItem } from "./NavItem";
import { NavLink } from "react-router-dom";

interface INavMenuProps {
  shouldCloseNavItem: (event: MouseEvent) => boolean;
}

interface INavMenuState {
  screenWidth: number;
  selectedNavId: string | null;
}

const maxResponsiveWidth = 798;

export class NavMenu extends React.Component<INavMenuProps, INavMenuState> {
  private navRef: HTMLElement | null = null;
  constructor(props: any) {
    super(props);
    this.selectNavItem = this.selectNavItem.bind(this);
    this.setNavRef = this.setNavRef.bind(this);
    this.handleClickOutside = this.handleClickOutside.bind(this);
    this.updateWindowDimensions = this.updateWindowDimensions.bind(this);
  }

  public componentDidMount() {
    // because this is loaded isomorphically we must wait until the client has loaded to check this
    // rather than in setting the initial state
    this.updateWindowDimensions();
    window.addEventListener("resize", this.updateWindowDimensions);
    document.addEventListener("mousedown", this.handleClickOutside);
  }

  public componentWillUnmount() {
    window.removeEventListener("resize", this.updateWindowDimensions);
    document.removeEventListener("mousedown", this.handleClickOutside);
  }

  private isMobile(): boolean {
    return this.state.screenWidth <= maxResponsiveWidth;
  }

  private updateWindowDimensions() {
    global.console.log("RESIZE WINDOW????");
    // TODO - setup global react hook for this?
    this.setState({ screenWidth: window.innerWidth });
  }

  private setNavRef = (node: HTMLElement) => {
    this.navRef = node;
  };

  private handleClickOutside = (event: MouseEvent) => {
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    if (
      this.navRef &&
      !this.navRef.contains(event.target as any) &&
      this.props.shouldCloseNavItem(event)
    ) {
      this.setState({ selectedNavId: null });
    }
  };

  public state: INavMenuState = {
    screenWidth: 0,
    selectedNavId: null
  };

  private selectNavItem = (navId: string, hasChildren: boolean) => {
    // don't collapse nav when clicked on if in mobile view (but collapse sub-nav if parent clicked)
    if(!hasChildren && this.isMobile()) return;
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
      <nav ref={this.setNavRef}>
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
