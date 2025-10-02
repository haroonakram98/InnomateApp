import React from "react";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  children: React.ReactNode;
  loading?: boolean;
}

const Button: React.FC<ButtonProps> = ({ children, loading, ...props }) => {
  return (
    <button
      {...props}
      disabled={loading || props.disabled}
      className="w-full px-4 py-2 bg-blue-600 text-white rounded-lg 
                 hover:bg-blue-700 transition-colors disabled:opacity-50"
    >
      {loading ? "Please wait..." : children}
    </button>
  );
};

export default Button;