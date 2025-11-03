import React from "react";

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
}

const Input: React.FC<InputProps> = ({ label, error, ...props }) => {
  return (
    <div className="flex flex-col w-full">
      <label
        className="mb-1 text-sm font-medium text-gray-700 dark:text-gray-300"
        htmlFor={props.id}
      >
        {label}
      </label>
      <input
        {...props}
        className={`px-3 py-2 rounded-lg border w-full text-sm sm:text-base
          bg-white dark:bg-gray-800
          border-gray-300 dark:border-gray-600
          text-gray-900 dark:text-gray-100
          placeholder-gray-400 dark:placeholder-gray-500
          focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
          transition-all duration-200
          disabled:opacity-50 disabled:cursor-not-allowed`}
      />
      {error && (
        <span className="text-xs text-red-500 dark:text-red-400 mt-1">{error}</span>
      )}
    </div>
  );
};

export default Input;
