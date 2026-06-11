import { SidebarProvider, Sidebar, SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuItem, SidebarMenuButton } from "#components/ui/sidebar";
import { LogOut, Utensils, Scale, User, Sparkles, Calendar, Dumbbell } from "lucide-react";

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher' | 'workout';

interface Props {
  children: React.ReactNode;
  onLogout: () => void;
  activePage: Page;
  onNavigate: (page: Page) => void;
}

const navItems: { page: Page; icon: React.ElementType; label: string }[] = [
  { page: 'diary',        icon: Utensils, label: 'Dziennik' },
  { page: 'calendar',     icon: Calendar, label: 'Kalendarz' },
  { page: 'ai-matcher',   icon: Sparkles, label: 'AI' },
  { page: 'workout',      icon: Dumbbell, label: 'Siłownia' },
  { page: 'measurements', icon: Scale,    label: 'Pomiary' },
  { page: 'profile',      icon: User,     label: 'Profil' },
];

export function AppLayout({ children, onLogout, activePage, onNavigate }: Props) {
  return (
    <SidebarProvider>
      <div className="flex min-h-screen w-full bg-slate-50 font-sans text-slate-900">

        {/* Sidebar — widoczny tylko na desktopie */}
        <Sidebar className="border-r border-slate-200 bg-white shadow-sm">
          <SidebarContent className="py-6">
            <SidebarGroup>
              <SidebarMenu className="space-y-2 px-4">

                {navItems.map(({ page, icon: Icon, label }) => (
                  <SidebarMenuItem key={page}>
                    <SidebarMenuButton
                      onClick={() => onNavigate(page)}
                      className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === page ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}
                    >
                      <Icon className={`mr-2 h-5 w-5 ${page === 'ai-matcher' ? 'text-indigo-500' : ''}`} />
                      <span className="text-base">{page === 'ai-matcher' ? 'Dobór Posiłku AI' : label}</span>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}

                <SidebarMenuItem>
                  <SidebarMenuButton
                    className="w-full text-slate-600 hover:bg-red-50 hover:text-red-600 transition-colors rounded-lg py-6 mt-4 font-medium"
                    onClick={onLogout}
                  >
                    <LogOut className="mr-2 h-5 w-5" />
                    <span className="text-base">Wyloguj</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>

              </SidebarMenu>
            </SidebarGroup>
          </SidebarContent>
        </Sidebar>

        {/* Obszar główny */}
        <div className="flex flex-1 flex-col min-w-0">
          {/* Mobilny nagłówek */}
          <header className="flex items-center justify-between h-14 px-4 border-b border-slate-200 bg-white md:hidden shrink-0">
            <span className="font-bold text-slate-800 text-lg">FitApp</span>
            <button
              onClick={onLogout}
              className="flex items-center gap-1.5 text-sm text-slate-500 hover:text-red-600 transition-colors"
            >
              <LogOut className="h-4 w-4" />
              Wyloguj
            </button>
          </header>

          <main className="flex-1 p-4 md:p-8 overflow-x-hidden pb-20 md:pb-8">
            {children}
          </main>
        </div>

        {/* Dolna nawigacja — tylko mobile */}
        <nav className="fixed bottom-0 left-0 right-0 z-50 bg-white border-t border-slate-200 md:hidden">
          <div className="flex">
            {navItems.map(({ page, icon: Icon, label }) => (
              <button
                key={page}
                onClick={() => onNavigate(page)}
                className={`flex-1 flex flex-col items-center justify-center py-2 gap-0.5 text-[10px] font-medium transition-colors ${
                  activePage === page
                    ? 'text-blue-600'
                    : 'text-slate-400 hover:text-slate-600'
                }`}
              >
                <Icon className={`h-5 w-5 ${page === 'ai-matcher' && activePage !== 'ai-matcher' ? 'text-indigo-400' : ''}`} />
                <span>{label}</span>
              </button>
            ))}
          </div>
        </nav>

      </div>
    </SidebarProvider>
  );
}
