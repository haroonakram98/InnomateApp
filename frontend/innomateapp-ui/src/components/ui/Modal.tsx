import React, { useEffect } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { X } from "lucide-react";

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  size?: "sm" | "md" | "lg" | "xl";
  showFooter?: boolean;
  footerContent?: React.ReactNode;
  children: React.ReactNode;
}

const sizeClasses = {
  sm: "max-w-sm",
  md: "max-w-md",
  lg: "max-w-2xl",
  xl: "max-w-4xl",
};

const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  title,
  size = "lg",
  showFooter = false,
  footerContent,
  children,
}) => {
  // Close on ESC key
  useEffect(() => {
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    if (isOpen) document.addEventListener("keydown", handler);
    return () => document.removeEventListener("keydown", handler);
  }, [isOpen, onClose]);

  // Lock body scroll when modal open
  useEffect(() => {
    document.body.style.overflow = isOpen ? "hidden" : "";
  }, [isOpen]);

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm"
          onClick={onClose}
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
        >
          <motion.div
            className={`bg-white dark:bg-gray-900 rounded-2xl shadow-2xl w-full ${sizeClasses[size]} mx-4 relative`}
            initial={{ scale: 0.9, opacity: 0, y: 20 }}
            animate={{ scale: 1, opacity: 1, y: 0 }}
            exit={{ scale: 0.9, opacity: 0, y: 20 }}
            transition={{ duration: 0.2 }}
            onClick={(e) => e.stopPropagation()} // prevent close on inner click
          >
            {/* Header */}
            <div className="flex items-center justify-between border-b px-4 py-3">
              <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100">
                {title ?? ""}
              </h2>
              <button
                onClick={onClose}
                className="p-1.5 rounded-full hover:bg-gray-200 dark:hover:bg-gray-700"
              >
                <X size={18} />
              </button>
            </div>

            {/* Content */}
            <div className="p-4 max-h-[80vh] overflow-y-auto">
              {children}
            </div>

            {/* Footer (optional) */}
            {showFooter && (
              <div className="border-t px-4 py-3 bg-gray-50 dark:bg-gray-800 rounded-b-2xl">
                {footerContent}
              </div>
            )}
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
};

export default Modal;
