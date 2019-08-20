import * as React from "react";

interface INavItemProps {
  navId: string;
  navTitle: string;
  selectedNavId: string | null;
  selectNavItem: (navId: string, hasChildren: boolean) => void;
}

interface INavItemState {
  subNavOpen: boolean;
}

export class NavItem extends React.Component<INavItemProps, INavItemState> {
  private wrapperRef: any;
  constructor(props: any) {
    super(props);
    this.subNavClickHandler = this.subNavClickHandler.bind(this);
  }

  public state: INavItemState = {
    subNavOpen: false
  };

  private navClickHandler = () => {
    this.props.selectNavItem(this.props.navId, false);
  };

  private subNavClickHandler = () => {
    this.props.selectNavItem(this.props.navId, true);
  }

  public render() {
    const subNavItems = React.Children.map(this.props.children, child => {
      return <li>{child}</li>;
    });

    const isSubNavShown =
      this.props.navId === this.props.selectedNavId
        ? { display: "block" }
        : { display: "none" };

    // Note react requires unique key to be set with array of child elements (e.g. see below)
    const navItem =
      subNavItems.length > 1
        ? [
            <a onClick={this.subNavClickHandler} key="1">{this.props.navTitle}</a>,
            this.props.children && (
              <ul className="nav-dropdown" style={isSubNavShown} key="2">
                {subNavItems}
              </ul>
            )
          ]
        : this.props.children;

    return <li onClick={this.navClickHandler}>{navItem}</li>;
  }
}
