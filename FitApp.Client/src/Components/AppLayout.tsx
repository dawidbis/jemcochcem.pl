import { LogOut, Utensils} from "lucide-react";

interface AppLayoutProps {
  children: React.ReactNode;
  onLogout: () => void;
  currentView: 'diary' | 'profile';
  setView: (view: 'diary' | 'profile') => void;
}

export function AppLayout({ children, onLogout, currentView, setView }: AppLayoutProps) {
  return (
    <div className="min-h-screen w-full bg-slate-50 font-sans text-slate-900 flex flex-col">
      <header className="bg-white border-b border-slate-200 shadow-sm px-6 py-4 flex justify-between items-center sticky top-0 z-10 w-full">
        <div className="flex items-center gap-8">
          <div className="flex items-center gap-2 text-blue-600">
            <Utensils className="h-6 w-6" />
            <span className="text-xl font-bold tracking-tight">FitApp</span>
          </div>
          
          <nav className="flex gap-1">
            <button 
              onClick={() => setView('diary')}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${currentView === 'diary' ? 'bg-blue-50 text-blue-700' : 'text-slate-500 hover:bg-slate-50'}`}
            >
              Dziennik
            </button>
            <button 
              onClick={() => setView('profile')}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${currentView === 'profile' ? 'bg-blue-50 text-blue-700' : 'text-slate-500 hover:bg-slate-50'}`}
            >
              Mój Profil
            </button>
          </nav>
        </div>

        <button onClick={onLogout} className="flex items-center gap-2 text-slate-400 hover:text-red-600 transition-colors font-medium">
          <LogOut className="h-5 w-5" />
          <span>Wyloguj</span>
        </button>
      </header>
      
      <main className="flex-1 p-4 md:p-6 lg:p-8 w-full mx-auto">
        {children}
      </main>
    </div>
  );
}