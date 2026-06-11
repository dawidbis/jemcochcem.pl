import { LogOut, Utensils, Scale, User, Sparkles, Calendar, Dumbbell } from "lucide-react";
import { useIsMobile } from "#hooks/use-mobile";

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher' | 'workout';

interface Props {
  children: React.ReactNode;
  onLogout: () => void;
  activePage: Page;
  onNavigate: (page: Page) => void;
}

const navItems: { page: Page; icon: React.ElementType; label: string; desktopLabel?: string }[] = [
  { page: 'diary',        icon: Utensils, label: 'Dziennik' },
  { page: 'calendar',     icon: Calendar, label: 'Kalendarz' },
  { page: 'ai-matcher',   icon: Sparkles, label: 'AI',       desktopLabel: 'Dobór Posiłku AI' },
  { page: 'workout',      icon: Dumbbell, label: 'Siłownia' },
  { page: 'measurements', icon: Scale,    label: 'Pomiary' },
  { page: 'profile',      icon: User,     label: 'Profil' },
];

export function AppLayout({ children, onLogout, activePage, onNavigate }: Props) {
  const isMobile = useIsMobile();

  return (
    <div style={{ display: 'flex', minHeight: '100vh', width: '100%', backgroundColor: '#f8fafc', fontFamily: 'sans-serif', color: '#0f172a' }}>

      {/* Boczny pasek — tylko desktop */}
      {!isMobile && (
        <aside style={{ width: '16rem', minHeight: '100vh', borderRight: '1px solid #e2e8f0', backgroundColor: '#ffffff', boxShadow: '1px 0 4px rgba(0,0,0,0.05)', display: 'flex', flexDirection: 'column', padding: '1.5rem 1rem', flexShrink: 0 }}>
          {navItems.map(({ page, icon: Icon, label, desktopLabel }) => (
            <button
              key={page}
              onClick={() => onNavigate(page)}
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
              {desktopLabel ?? label}
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
        </aside>
      )}

      {/* Główna zawartość */}
      <div style={{ flex: 1, display: 'flex', flexDirection: 'column', minWidth: 0 }}>

        {/* Nagłówek mobilny */}
        {isMobile && (
          <header style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', height: '3.5rem', padding: '0 1rem', borderBottom: '1px solid #e2e8f0', backgroundColor: '#ffffff', flexShrink: 0 }}>
            <span style={{ fontWeight: 700, fontSize: '1.125rem', color: '#1e293b' }}>FitApp</span>
            <button onClick={onLogout} style={{ display: 'flex', alignItems: 'center', gap: '0.375rem', fontSize: '0.875rem', color: '#64748b', background: 'none', border: 'none', cursor: 'pointer' }}>
              <LogOut size={16} />
              Wyloguj
            </button>
          </header>
        )}

        <main style={{ flex: 1, padding: isMobile ? '1rem' : '2rem', overflowX: 'hidden', paddingBottom: isMobile ? '5rem' : undefined }}>
          {children}
        </main>
      </div>

      {/* Dolna nawigacja — tylko mobile */}
      {isMobile && (
        <nav style={{ position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 9999, backgroundColor: '#ffffff', borderTop: '1px solid #e2e8f0', display: 'flex' }}>
          {navItems.map(({ page, icon: Icon, label }) => (
            <button
              key={page}
              onClick={() => onNavigate(page)}
              style={{
                flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center',
                justifyContent: 'center', padding: '0.5rem 0.25rem', gap: '0.125rem',
                fontSize: '0.625rem', fontWeight: 500, border: 'none', cursor: 'pointer',
                backgroundColor: 'transparent',
                color: activePage === page ? '#2563eb' : '#94a3b8',
              }}
            >
              <Icon size={20} style={{ color: page === 'ai-matcher' && activePage !== page ? '#818cf8' : 'inherit' }} />
              <span>{label}</span>
            </button>
          ))}
        </nav>
      )}

    </div>
  );
}
