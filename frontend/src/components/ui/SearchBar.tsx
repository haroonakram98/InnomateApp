import React from "react";

interface SearchBarProps {
  onChange: (value: string) => void;
}

export const SearchBar: React.FC<SearchBarProps> = ({ onChange }) => (
  <input
    type="text"
    placeholder="Search categories..."
    onChange={(e) => onChange(e.target.value)}
    className="w-full rounded-md bg-gray-800 px-3 py-2 text-sm placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-600"
  />
);
