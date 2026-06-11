import { SidebarProvider, SidebarTrigger, Sidebar, SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuItem, SidebarMenuButton, useSidebar } from "#components/ui/sidebar";
import { LogOut, Utensils, Scale, User, Sparkles, Calendar, Dumbbell } from "lucide-react";

type Page = 'diary' | 'calendar' | 'measurements' | 'profile' | 'ai-matcher' | 'workout';

interface Props {
  children: React.ReactNode;
  onLogout: () => void;
  activePage: Page;
  onNavigate: (page: Page) => void;
}

interface NavProps {
  activePage: Page;
  onNavigate: (page: Page) => void;
  onLogout: () => void;
}

function SidebarNav({ activePage, onNavigate, onLogout }: NavProps) {
  const { setOpenMobile, isMobile } = useSidebar();

  const navigate = (page: Page) => {
    onNavigate(page);
    if (isMobile) setOpenMobile(false);
  };

  return (
    <SidebarContent className="py-6">
      <SidebarGroup>
        <SidebarMenu className="space-y-2 px-4">

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('diary')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'diary' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <Utensils className="mr-2 h-5 w-5" />
              <span className="text-base">Dziennik</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('calendar')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'calendar' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <Calendar className="mr-2 h-5 w-5" />
              <span className="text-base">Kalendarz diety</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('ai-matcher')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'ai-matcher' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <Sparkles className="mr-2 h-5 w-5 text-indigo-500" />
              <span className="text-base font-semibold">Dobór Posiłku AI</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('workout')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'workout' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <Dumbbell className="mr-2 h-5 w-5" />
              <span className="text-base">Siłownia</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('measurements')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'measurements' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <Scale className="mr-2 h-5 w-5" />
              <span className="text-base">Pomiary</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => navigate('profile')} className={`w-full transition-colors rounded-lg py-6 font-medium ${activePage === 'profile' ? 'bg-blue-50 text-blue-700 hover:bg-blue-100' : 'text-slate-600 hover:bg-slate-100'}`}>
              <User className="mr-2 h-5 w-5" />
              <span className="text-base">Mój Profil</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

          <SidebarMenuItem>
            <SidebarMenuButton className="w-full text-slate-600 hover:bg-red-50 hover:text-red-600 transition-colors rounded-lg py-6 mt-4 font-medium" onClick={onLogout}>
              <LogOut className="mr-2 h-5 w-5" />
              <span className="text-base">Wyloguj</span>
            </SidebarMenuButton>
          </SidebarMenuItem>

        </SidebarMenu>
      </SidebarGroup>
    </SidebarContent>
  );
}

export function AppLayout({ children, onLogout, activePage, onNavigate }: Props) {
  return (
    <SidebarProvider>
      <div className="flex min-h-screen w-full bg-slate-50 font-sans text-slate-900">
        <Sidebar className="border-r border-slate-200 bg-white shadow-sm">
          <SidebarNav activePage={activePage} onNavigate={onNavigate} onLogout={onLogout} />
        </Sidebar>
        <div className="flex flex-1 flex-col min-w-0">
          <header className="flex items-center h-14 px-4 border-b border-slate-200 bg-white md:hidden shrink-0">
            <SidebarTrigger className="mr-3 text-slate-600" />
            <span className="font-semibold text-slate-800 text-base">FitApp</span>
          </header>
          <main className="flex-1 p-4 md:p-8 overflow-x-hidden">
            {children}
          </main>
        </div>
      </div>
    </SidebarProvider>
  );
}
