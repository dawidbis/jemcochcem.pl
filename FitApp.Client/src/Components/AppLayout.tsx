import { useState } from "react";
import { LogOut, Utensils, Scale, User, Sparkles, Calendar, Dumbbell, Menu, X } from "lucide-react";
import { useIsMobile } from "#hooks/use-mobile";

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher' | 'workout';

interface Props {
  children: React.ReactNode;
  onLogout: () => void;
  activePage: Page;
  onNavigate: (page: Page) => void;
}

const navItems: { page: Page; icon: React.ElementType; label: string }[] = [
  { page: 'diary',        icon: Utensils, label: 'Dziennik' },
  { page: 'calendar',     icon: Calendar, label: 'Kalendarz diety' },
  { page: 'ai-matcher',   icon: Sparkles, label: 'Dobór Posiłku AI' },
  { page: 'workout',      icon: Dumbbell, label: 'Siłownia' },
  { page: 'measurements', icon: Scale,    label: 'Pomiary' },
  { page: 'profile',      icon: User,     label: 'Mój Profil' },
];

function SidebarContent({ activePage, onNavigate, onLogout, onClose }: {
  activePage: Page;
  onNavigate: (page: Page) => void;
  onLogout: () => void;
  onClose?: () => void;
}) {
  const navigate = (page: Page) => {
    onNavigate(page);
    onClose?.();
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%', padding: '1.5rem 1rem' }}>
      {navItems.map(({ page, icon: Icon, label }) => (
        <button
          key={page}
          onClick={() => navigate(page)}
          style={{
            display: 'flex', alignItems: 'center', gap: '0.625rem',
            padding: '0.75rem 1rem', borderRadius: '0.5rem', border: 'none',
            cursor: 'pointer', width: '100%', textAlign: 'left',
            marginBottom: '0.25rem', fontWeight: 500, fontSize: '0.9375rem',
            backgroundColor: activePage === page ? '#eff6ff' : 'transparent',
            color: activePage === page ? '#1d4ed8' : '#475569',
          }}
        >
          <Icon size={18} style={{ color: page === 'ai-matcher' ? '#6366f1' : 'inherit', flexShrink: 0 }} />
          {label}
        </button>
      ))}

      <button
        onClick={onLogout}
        style={{
          display: 'flex', alignItems: 'center', gap: '0.625rem',
          padding: '0.75rem 1rem', borderRadius: '0.5rem', border: 'none',
          cursor: 'pointer', width: '100%', textAlign: 'left',
          marginTop: 'auto', fontWeight: 500, fontSize: '0.9375rem',
          backgroundColor: 'transparent', color: '#64748b',
        }}
      >
        <LogOut size={18} style={{ flexShrink: 0 }} />
        Wyloguj
      </button>
    </div>
  );
}

export function AppLayout({ children, onLogout, activePage, onNavigate }: Props) {
  const isMobile = useIsMobile();
  const [drawerOpen, setDrawerOpen] = useState(false);

  return (
    <div style={{ display: 'flex', minHeight: '100vh', width: '100%', backgroundColor: '#f8fafc', fontFamily: 'sans-serif', color: '#0f172a' }}>

      {/* Boczny pasek — desktop */}
      {!isMobile && (
        <aside style={{ width: '16rem', minHeight: '100vh', borderRight: '1px solid #e2e8f0', backgroundColor: '#ffffff', boxShadow: '1px 0 4px rgba(0,0,0,0.05)', flexShrink: 0 }}>
          <SidebarContent activePage={activePage} onNavigate={onNavigate} onLogout={onLogout} />
        </aside>
      )}

      {/* Główna zawartość */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Nagłówek mobilny z hamburgerem */}
        {isMobile && (
          <header style={{ display: 'flex', alignItems: 'center', height: '3.5rem', padding: '0 1rem', borderBottom: '1px solid #e2e8f0', backgroundColor: '#ffffff', flexShrink: 0, gap: '0.75rem' }}>
            <button
              onClick={() => setDrawerOpen(true)}
              style={{ background: 'none', border: 'none', cursor: 'pointer', padding: '0.375rem', color: '#475569', display: 'flex', alignItems: 'center' }}
            >
              <Menu size={24} />
            </button>
            <span style={{ fontWeight: 700, fontSize: '1.125rem', color: '#1e293b' }}>FitApp</span>
          </header>
        )}

        <main style={{ flex: 1, padding: isMobile ? '1rem' : '2rem', overflowX: 'hidden' }}>
          {children}
        </main>
      </div>

      {/* Drawer — mobile */}
      {isMobile && (
        <>
          {/* Overlay */}
          <div
            onClick={() => setDrawerOpen(false)}
            style={{
              position: 'fixed', inset: 0, zIndex: 9998,
              backgroundColor: 'rgba(0,0,0,0.4)',
              opacity: drawerOpen ? 1 : 0,
              pointerEvents: drawerOpen ? 'auto' : 'none',
              transition: 'opacity 0.25s ease',
            }}
          />

          {/* Panel boczny */}
          <div
            style={{
              position: 'fixed', top: 0, left: 0, bottom: 0, zIndex: 9999,
              width: '17rem', backgroundColor: '#ffffff',
              boxShadow: '4px 0 16px rgba(0,0,0,0.12)',
              transform: drawerOpen ? 'translateX(0)' : 'translateX(-100%)',
              transition: 'transform 0.25s ease',
              display: 'flex', flexDirection: 'column',
            }}
          >
            {/* Nagłówek drawera */}
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 1rem', height: '3.5rem', borderBottom: '1px solid #e2e8f0', flexShrink: 0 }}>
              <span style={{ fontWeight: 700, fontSize: '1.125rem', color: '#1e293b' }}>FitApp</span>
              <button
                onClick={() => setDrawerOpen(false)}
                style={{ background: 'none', border: 'none', cursor: 'pointer', padding: '0.375rem', color: '#64748b', display: 'flex', alignItems: 'center' }}
              >
                <X size={20} />
              </button>
            </div>

            <SidebarContent
              activePage={activePage}
              onNavigate={onNavigate}
              onLogout={onLogout}
              onClose={() => setDrawerOpen(false)}
            />
          </div>
        </>
      )}

    </div>
  );
}
