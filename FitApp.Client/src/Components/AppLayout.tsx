<<<<<<< HEAD
import { SidebarProvider, Sidebar, SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuItem, SidebarMenuButton } from "#components/ui/sidebar";
import { LogOut, Utensils, Scale, User, CalendarDays  } from "lucide-react";

type Page = 'diary' |'calendar'| 'measurements' | 'profile';
=======
import { SidebarProvider, Sidebar, SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuItem, SidebarMenuButton } from "#components/ui/sidebar";//nie wiem o o co chodzi nie zmieniałem tego jak co a jest błąd tutaj u mnie - mf 
import { LogOut, Utensils, Scale, User,Sparkles } from "lucide-react";

type Page = 'diary' | 'measurements' | 'profile'| 'ai-matcher';
>>>>>>> 31c9c58 (Zapisuje front przed pullem AI)

interface Props {
  children: React.ReactNode;
  onLogout: () => void;
  activePage: Page;
  onNavigate: (page: Page) => void;
}

export function AppLayout({ children, onLogout, activePage, onNavigate }: Props) {
  return (
   <SidebarProvider>
      <div className="flex min-h-screen w-full bg-slate-50 font-sans text-slate-900">
        <Sidebar className="border-r border-slate-200 bg-white shadow-sm">
          <SidebarContent className="py-6">
            <SidebarGroup>
              <SidebarMenu className="space-y-2 px-4">
                <SidebarMenuItem>
                  <SidebarMenuButton
                    onClick={() => onNavigate('diary')}
                    className={`w-full transition-colors rounded-lg py-6 font-medium ${
                      activePage === 'diary'
                        ? 'bg-blue-50 text-blue-700 hover:bg-blue-100'
                        : 'text-slate-600 hover:bg-slate-100'
                    }`}
                  >
                    <Utensils className="mr-2 h-5 w-5" />
                    <span className="text-base">Dziennik</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarMenuButton
<<<<<<< HEAD
                    onClick={() => onNavigate('calendar')}
                    className={`w-full transition-colors rounded-lg py-6 font-medium ${
                      activePage === 'calendar'
                        ? 'bg-blue-50 text-blue-700 hover:bg-blue-100'
                        : 'text-slate-600 hover:bg-slate-100'
                    }`}
                  >
                    <CalendarDays className="mr-2 h-5 w-5" />
                    <span className="text-base">Kalendarz</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>
=======
                  onClick={() => onNavigate('ai-matcher')}
                  className={`w-full transition-colors rounded-lg py-6 font-medium ${
                    activePage === 'ai-matcher'
                      ? 'bg-blue-50 text-blue-700 hover:bg-blue-100'
                      : 'text-slate-600 hover:bg-slate-100'
                  }`}
                >
                  <Sparkles className="mr-2 h-5 w-5 text-indigo-500" />
                  <span className="text-base font-semibold">Dobór Posiłku AI</span>
                </SidebarMenuButton>
              </SidebarMenuItem>
>>>>>>> 31c9c58 (Zapisuje front przed pullem AI)
                <SidebarMenuItem>
                  <SidebarMenuButton
                    onClick={() => onNavigate('measurements')}
                    className={`w-full transition-colors rounded-lg py-6 font-medium ${
                      activePage === 'measurements'
                        ? 'bg-blue-50 text-blue-700 hover:bg-blue-100'
                        : 'text-slate-600 hover:bg-slate-100'
                    }`}
                  >
                    <Scale className="mr-2 h-5 w-5" />
                    <span className="text-base">Pomiary</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>
                <SidebarMenuItem>
                  <SidebarMenuButton
                    onClick={() => onNavigate('profile')}
                    className={`w-full transition-colors rounded-lg py-6 font-medium ${
                      activePage === 'profile'
                        ? 'bg-blue-50 text-blue-700 hover:bg-blue-100'
                        : 'text-slate-600 hover:bg-slate-100'
                    }`}
                  >
                    <User className="mr-2 h-5 w-5" />
                    <span className="text-base">Mój Profil</span>
                  </SidebarMenuButton>
                </SidebarMenuItem>
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
        <main className="flex-1 p-4 md:p-8 overflow-x-hidden">
          {children}
        </main>
      </div>
    </SidebarProvider>
  );
}
