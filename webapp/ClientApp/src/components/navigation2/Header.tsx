import * as React from "react";
import { NavMenu } from "./NavMenu";

interface IToolbarProps {
  mobileNavToggleHandler: (isMobileNavOpen: boolean) => void;
}

interface IHeaderState {
  isMobileNavOpen: boolean;
}

export class Header extends React.Component<IToolbarProps, IHeaderState> {
  private navMobileToggleRef: HTMLAnchorElement | null = null;
  private navRef: HTMLElement | null = null;
  constructor(props: any) {
    super(props);
    this.shouldCloseNavItem = this.shouldCloseNavItem.bind(this);
    this.toggleMobileNav = this.toggleMobileNav.bind(this);
    this.handleClickOutsideMobileNav = this.handleClickOutsideMobileNav.bind(this);
  }

  public componentDidMount() {
    // because this is loaded isomorphically we must wait until the client has loaded to check this
    // rather than in setting the initial state
    document.addEventListener("mousedown", this.handleClickOutsideMobileNav);
  }

  public componentWillUnmount() {
    // TODO - use react hooks for this?
    document.removeEventListener("mousedown", this.handleClickOutsideMobileNav);
  }

  public state: IHeaderState = {
    isMobileNavOpen: false
  };

  private shouldCloseNavItem(event: MouseEvent): boolean {
    return (
      this.navMobileToggleRef !== null &&
      !this.navMobileToggleRef.contains(event.target as any)
    );
  }

  private toggleMobileNav = () => {
    const isMobileNavOpen = !this.state.isMobileNavOpen;
    this.setState({ isMobileNavOpen });
    this.props.mobileNavToggleHandler(isMobileNavOpen);
  };

  private setNavMobileToggleRef = (node: HTMLAnchorElement) => {
    this.navMobileToggleRef = node;
  };

  private setNavRef = (node: HTMLElement) => {
    this.navRef = node;
  }

  private handleClickOutsideMobileNav = (event: MouseEvent) => {
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    if (
      this.navRef &&
      !this.navRef.contains(event.target as any)
    ) {
      this.setState({ isMobileNavOpen: false });
      this.props.mobileNavToggleHandler(false); // TODO - centralize logic here
    }
  };

  public render() {
    const mobileClass = this.state.isMobileNavOpen ? "active" : "";
    return (
      <section ref={this.setNavRef} className="navigation">
        <div className="nav-container">
          <div className="nav-top">
            <div className="brand">
              <a href="#!">Logo</a>
            </div>
            <div className="nav-mobile">
              <a
                ref={this.setNavMobileToggleRef}
                onClick={this.toggleMobileNav}
                id="nav-toggle"
                className={mobileClass}
                href="#!"
              >
                <span />
              </a>
            </div>
          </div>
          <NavMenu
            shouldCloseNavItem={this.shouldCloseNavItem}
            isMobileNavOpen={this.state.isMobileNavOpen}
          />
        </div>
      </section>
    );
  }
}
