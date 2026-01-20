import React, { useEffect } from 'react';
import { useToastStore } from '@/store/useToastStore';

const ToastItem: React.FC<{id:number; message:string; type?:string; onClose:()=>void}> = ({id, message, type, onClose}) => {
  useEffect(() => {
    const t = setTimeout(onClose, 3500);
    return () => clearTimeout(t);
  }, [onClose]);
  const bg = type === 'success' ? 'bg-green-500' : type === 'error' ? 'bg-red-500' : 'bg-gray-700';
  return (
    <div className={`text-white px-4 py-2 rounded shadow ${bg}`}>
      {message}
    </div>
  );
};

const ToastContainer: React.FC = () => {
  const { toasts, remove } = useToastStore();
  return (
    <div className="fixed right-4 bottom-4 flex flex-col gap-2 z-50">
      {toasts.map(t => (
        <div key={t.id}>
          <ToastItem id={t.id} message={t.message} type={t.type} onClose={() => remove(t.id)} />
        </div>
      ))}
    </div>
  );
};

export default ToastContainer;
