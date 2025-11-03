import React from "react";
import clsx from "clsx";

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {}

export const Card: React.FC<CardProps> = ({ children, className, ...props }) => (
  <div
    {...props}
    className={clsx(
      "bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100",
      "rounded-2xl shadow-sm border border-gray-200 dark:border-gray-700",
      "hover:shadow-md transition-all duration-200 ease-in-out",
      className
    )}
  >
    {children}
  </div>
);

interface CardContentProps extends React.HTMLAttributes<HTMLDivElement> {}

export const CardContent: React.FC<CardContentProps> = ({
  children,
  className,
  ...props
}) => (
  <div {...props} className={clsx("p-4 sm:p-5", className)}>
    {children}
  </div>
);
