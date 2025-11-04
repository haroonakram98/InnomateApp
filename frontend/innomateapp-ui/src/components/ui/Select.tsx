import React from "react";

interface Option {
  value: string | number;
  label: string;
}

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  label?: string;
  options: Option[];
  error?: string;
  isLoading?: boolean;
}

export default function Select({
  label,
  options = [],
  error,
  isLoading = false,
  value,
  onChange,
  ...rest
}: SelectProps) {
  return (
    <div className="flex flex-col gap-1">
      {label && (
        <label className="text-sm font-medium text-gray-700 dark:text-gray-300">
          {label}
        </label>
      )}

      <div className="relative">
        <select
          value={value ?? ""}
          defaultValue={value ?? ""}
          onChange={onChange}
          disabled={isLoading}
          className={`w-full px-3 py-2 rounded-lg border text-gray-900 dark:text-gray-100 bg-white dark:bg-gray-700
            focus:ring-2 focus:ring-blue-500 outline-none
            ${error ? "border-red-500" : "border-gray-300 dark:border-gray-600"}
            ${isLoading ? "opacity-70 cursor-not-allowed" : ""}`}
          {...rest}
        >
          {/* Default option */}
          {options.length > 0 && options[0].value === 0 ? null : (
            <option value="">Select...</option>
          )}

          {options.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>

        {isLoading && (
          <span className="absolute right-3 top-2.5 text-xs text-gray-500">
            Loading...
          </span>
        )}
      </div>

      {error && <p className="text-sm text-red-500">{error}</p>}
    </div>
  );
}
