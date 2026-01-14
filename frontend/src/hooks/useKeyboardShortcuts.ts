import { useEffect } from 'react';

type Handlers = {
  onSave?: () => void;
  onNew?: () => void;
  onProductSelect?: () => void;
};

export function useKeyboardShortcuts(handlers: Handlers) {
  useEffect(() => {
    function onKey(e: KeyboardEvent) {

      // Ctrl+S or Cmd+S
      if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') {
        e.preventDefault();
        handlers.onSave?.();
      }

      // Alt+N for new
      if (e.altKey && e.key.toLowerCase() === 'n') {
        console.log('true')
        e.preventDefault();
        handlers.onNew?.();
      }
    }
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [handlers]);
}
