import React from "react";
import clsx from "clsx";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children: React.ReactNode;
  loading?: boolean;
  variant?: "primary" | "secondary" | "outline" | "danger" | "ghost";
  fullWidth?: boolean;
  size?: "sm" | "xs" | "icon";
}

const Button: React.FC<ButtonProps> = ({
  children,
  loading,
  variant = "primary",
  size = "sm",
  fullWidth = false,
  className,
  ...props
}) => {
  const base =
    "px-4 py-2 rounded-lg font-medium transition-colors duration-200 focus:outline-none focus:ring-2 disabled:opacity-50 disabled:cursor-not-allowed";

  const variants = {
    primary:
      "bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-300 " +
      "dark:bg-blue-500 dark:hover:bg-blue-600 dark:focus:ring-blue-800",
    secondary:
      "bg-gray-100 text-gray-800 hover:bg-gray-200 focus:ring-gray-200 " +
      "dark:bg-gray-700 dark:text-gray-100 dark:hover:bg-gray-600 dark:focus:ring-gray-600",
    outline:
      "border border-gray-300 text-gray-700 hover:bg-gray-100 focus:ring-gray-100 " +
      "dark:border-gray-600 dark:text-gray-200 dark:hover:bg-gray-700 dark:focus:ring-gray-700",
    danger:
      "bg-red-600 text-white hover:bg-red-700 focus:ring-red-300 " +
      "dark:bg-red-500 dark:hover:bg-red-600 dark:focus:ring-red-800",
    ghost:
    "bg-transparent text-gray-700 hover:bg-gray-100 focus:ring-gray-100 " +
    "dark:text-gray-200 dark:hover:bg-gray-700 dark:focus:ring-gray-700",
  };

  return (
    <button
      {...props}
      disabled={loading || props.disabled}
      className={clsx(
        base,
        variants[variant],
        fullWidth && "w-full",
        "text-sm sm:text-base",
        className
      )}
    >
      {loading ? "Please wait..." : children}
    </button>
  );
};

export default Button;
