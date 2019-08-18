import * as React from "react";
import { NavMenu } from "./NavMenu";

interface IToolbarProps {
  drawerClickHandler: () => void;
}

interface IHeaderState {
    sideDrawerOpen: boolean;
}

export class Header extends React.Component<IToolbarProps, IHeaderState> {

    constructor(props: any) {
      super(props);
    }

    public render() {
        const showStyle = { display: "none" };
        return (
            <section className="navigation">
                <div className="nav-container">
                    <div className="brand">
                        <a href="#!">Logo</a>
                    </div>
                    <div className="nav-mobile">
                        <a id="nav-toggle" href="#!"><span /></a>
                    </div>
                    <NavMenu drawerClickHandler={this.props.drawerClickHandler} />
                </div>
            </section>
        );
    }
}