import * as React from "react";
import { NavMenu } from "./NavMenu";
import { useClickOutside } from 'use-events';

interface IHeaderProps {
  mobileNavToggleHandler: (isMobileNavOpen: boolean) => void;
}

export function Header(props: IHeaderProps) {
  const [isMobileNavOpen, setIsMobileNavOpen] = React.useState(false);
  const headerRef = React.useRef<HTMLElement>(null);
  const navMobileToggleRef = React.useRef<HTMLAnchorElement>(null);
  const toggleMobileNav = () => { 
    const isOpen = !isMobileNavOpen;
    setIsMobileNavOpen(isOpen);
    props.mobileNavToggleHandler(isOpen);
  };

  useClickOutside([headerRef], event => {
    // close the mobile nav when clicked outside the header
    setIsMobileNavOpen(false);
    // we also allow check with the parent (shouldCloseNavItem) to see if anything should block it there (e.g. toggling mobile nav)
    props.mobileNavToggleHandler(false);
  });

  const shouldCloseNavItem = (event: MouseEvent): boolean => {
    return (
      navMobileToggleRef.current !== null &&
      !navMobileToggleRef.current.contains(event.target as any)
    );
  }
  const mobileClass = isMobileNavOpen ? "active" : "";
  
  return (
    <section ref={headerRef} className="navigation">
      <div className="nav-container">
        <div className="nav-top">
          <div className="brand">
            <a href="#!">Logo</a>
          </div>
          <div className="nav-mobile">
            <a
              ref={navMobileToggleRef}
              onClick={toggleMobileNav}
              id="nav-toggle"
              className={mobileClass}
              href="#!"
            >
              <span />
            </a>
          </div>
        </div>
        <NavMenu
          shouldCloseNavItem={shouldCloseNavItem}
          isMobileNavOpen={isMobileNavOpen}
        />
      </div>
    </section>
  );
}
