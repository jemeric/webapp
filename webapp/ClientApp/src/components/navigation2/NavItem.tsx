import * as React from "react";
import { NavLink } from "react-router-dom";

interface INavItemProps {
  navId: string;
  navTitle: string;
  selectedNavId: string | null;
  selectNavItem: (navId: string) => void;
}

interface INavItemState {
  subNavOpen: boolean;
}

export class NavItem extends React.Component<INavItemProps, INavItemState> {
  private wrapperRef: any;
  constructor(props: any) {
    super(props);
    this.subNavClickHandler = this.subNavClickHandler.bind(this);
    this.setWrapperRef = this.setWrapperRef.bind(this);
    this.handleClickOutside = this.handleClickOutside.bind(this);
  }

  public componentDidMount() {
    document.addEventListener('mousedown', this.handleClickOutside);
  }

  public componentWillUnmount() {
    document.removeEventListener('mousedown', this.handleClickOutside);
  }

//   componentDidMount() {
//     
//   }

//   componentWillUnmount() {
//     document.removeEventListener('mousedown', this.handleClickOutside);
//   }

  private setWrapperRef = (node: any) => {
    this.wrapperRef = node;
  };

  private handleClickOutside = (event: any) => {
    if (this.wrapperRef && !this.wrapperRef.contains(event.target)) {
      // alert("You clicked outside of me!");
      global.console.log("CLICK OUTSIDE OF ME");
    }
  };

  public state: INavItemState = {
    subNavOpen: false
  };

  private subNavClickHandler = () => {
    // this.setState({ subNavOpen: !this.state.subNavOpen });
    this.props.selectNavItem(this.props.navId);
  };

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
            <a key="1">{this.props.navTitle}</a>,
            this.props.children && (
              <ul className="nav-dropdown" style={isSubNavShown} key="2">
                {subNavItems}
              </ul>
            )
          ]
        : this.props.children;

    global.console.log("NavID: ", this.props.navId, this.props.selectedNavId);
    return (
      <li ref={this.setWrapperRef} onClick={this.subNavClickHandler}>
        {navItem}
      </li>
    );
  }
}
