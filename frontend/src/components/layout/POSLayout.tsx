// // components/layout/POSLayout.js
// import React from "react";

// export const POSLayout = ({ children, className = "" }) => {
//   return (
//     <div className={`flex h-full bg-gray-50 dark:bg-gray-900 ${className}`}>
//       {children}
//     </div>
//   );
// };

// export const POSLeftPanel = ({ children, className = "" }) => {
//   return (
//     <div className={`w-80 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 flex flex-col ${className}`}>
//       {children}
//     </div>
//   );
// };

// export const POSMainContent = ({ children, className = "" }) => {
//   return (
//     <div className={`flex-1 flex flex-col min-w-0 bg-white dark:bg-gray-800 ${className}`}>
//       {children}
//     </div>
//   );
// };

// export const POSSection = ({ title, children, className = "" }) => {
//   return (
//     <div className={`border-b border-gray-200 dark:border-gray-700 ${className}`}>
//       <div className="p-4">
//         <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-3">{title}</h2>
//         {children}
//       </div>
//     </div>
//   );
// };