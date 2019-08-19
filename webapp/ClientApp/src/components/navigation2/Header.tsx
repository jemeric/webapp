import * as React from "react";
import { NavMenu } from "./NavMenu";

interface IToolbarProps {
  drawerClickHandler: () => void;
}

interface IHeaderState {
  sideDrawerOpen: boolean;
}

export class Header extends React.Component<IToolbarProps, IHeaderState> {
  private navMobileToggleRef: HTMLAnchorElement | null = null;
  constructor(props: any) {
    super(props);
    this.shouldCloseNavItem = this.shouldCloseNavItem.bind(this);
  }

  private shouldCloseNavItem(event: MouseEvent): boolean {
    return (
      this.navMobileToggleRef !== null &&
      !this.navMobileToggleRef.contains(event.target as any)
    );
  }

  private setNavMobileToggleRef = (node: HTMLAnchorElement) => {
    this.navMobileToggleRef = node;
  };

  public render() {
    const showStyle = { display: "none" };
    return (
      <section className="navigation">
        <div className="nav-container">
          <div className="brand">
            <a href="#!">Logo</a>
          </div>
          <div className="nav-mobile">
            <a ref={this.setNavMobileToggleRef} id="nav-toggle" href="#!">
              <span />
            </a>
          </div>
          <NavMenu shouldCloseNavItem={this.shouldCloseNavItem} />
        </div>
      </section>
    );
  }
}
