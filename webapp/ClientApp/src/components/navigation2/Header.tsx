import * as React from "react";
import { NavMenu } from "./NavMenu";

interface IToolbarProps {
  drawerClickHandler: () => void;
}

interface IHeaderState {
  isMobileNavOpen: boolean;
}

export class Header extends React.Component<IToolbarProps, IHeaderState> {
  private navMobileToggleRef: HTMLAnchorElement | null = null;
  constructor(props: any) {
    super(props);
    this.shouldCloseNavItem = this.shouldCloseNavItem.bind(this);
    this.toggleMobileNav = this.toggleMobileNav.bind(this);
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
    this.setState({ isMobileNavOpen: !this.state.isMobileNavOpen });
  };

  private setNavMobileToggleRef = (node: HTMLAnchorElement) => {
    this.navMobileToggleRef = node;
  };

  public render() {
    const mobileClass = this.state.isMobileNavOpen ? "active" : "";
    return (
      <section className="navigation">
        <div className="nav-container">
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
          <NavMenu
            shouldCloseNavItem={this.shouldCloseNavItem}
            isMobileNavOpen={this.state.isMobileNavOpen}
          />
        </div>
      </section>
    );
  }
}
