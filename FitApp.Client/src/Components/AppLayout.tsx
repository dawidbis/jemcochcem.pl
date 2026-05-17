import { SidebarProvider, Sidebar, SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuItem, SidebarMenuButton } from "#components/ui/sidebar";
import { LogOut, Utensils } from "lucide-react";

export function AppLayout({ children, onLogout }: { children: React.ReactNode, onLogout: () => void }) {
  return (
    <SidebarProvider>
      <div className="flex min-h-screen w-full bg-slate-50 font-sans text-slate-900">
        <Sidebar className="border-r border-slate-200 bg-white shadow-sm">
          <SidebarContent className="py-6">
            <SidebarGroup>
              <SidebarMenu className="space-y-2 px-4">
                <SidebarMenuItem>
                  <SidebarMenuButton className="w-full bg-blue-50 text-blue-700 hover:bg-blue-100 transition-colors rounded-lg py-6 font-medium">
                    <Utensils className="mr-2 h-5 w-5" />
                    <span className="text-base">Dziennik</span>
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
        </Sidebar>
        <main className="flex-1 p-4 md:p-8 overflow-x-hidden">
          {children}
        </main>
      </div>
    </SidebarProvider>
  );
}