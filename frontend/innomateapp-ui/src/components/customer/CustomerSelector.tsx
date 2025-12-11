// // components/customers/CustomerSelector.js
// import React, { useState } from "react";
// import { Search, User, UserPlus } from "lucide-react";
// import { motion, AnimatePresence } from "framer-motion";

// export const CustomerSelector = ({ 
//   customers = [], 
//   selectedCustomer, 
//   onSelectCustomer,
//   onAddNewCustomer,
//   className = "" 
// }) => {
//   const [searchQuery, setSearchQuery] = useState("");
//   const [isDropdownOpen, setIsDropdownOpen] = useState(false);

//   const filteredCustomers = customers.filter(customer =>
//     customer.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
//     customer.phone?.includes(searchQuery)
//   );

//   const handleSelectCustomer = (customer) => {
//     onSelectCustomer(customer);
//     setIsDropdownOpen(false);
//     setSearchQuery("");
//   };

//   return (
//     <div className={`relative ${className}`}>
//       <div className="flex items-center gap-3">
//         <div className="flex-1 relative">
//           <input
//             type="text"
//             placeholder="Search by name or contact..."
//             value={searchQuery}
//             onChange={(e) => {
//               setSearchQuery(e.target.value);
//               setIsDropdownOpen(true);
//             }}
//             onFocus={() => setIsDropdownOpen(true)}
//             className="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
//           />
//           <Search className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={16} />
//         </div>
        
//         <button 
//           onClick={onAddNewCustomer}
//           className="p-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors"
//           title="Add New Customer"
//         >
//           <UserPlus size={20} />
//         </button>
//       </div>

//       {/* Customer Dropdown */}
//       <AnimatePresence>
//         {isDropdownOpen && searchQuery && (
//           <motion.div
//             initial={{ opacity: 0, y: -10 }}
//             animate={{ opacity: 1, y: 0 }}
//             exit={{ opacity: 0, y: -10 }}
//             className="absolute top-full left-0 right-0 mt-1 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg z-10 max-h-60 overflow-y-auto"
//           >
//             {filteredCustomers.length === 0 ? (
//               <div className="p-3 text-center text-gray-500 dark:text-gray-400">
//                 No customers found
//               </div>
//             ) : (
//               filteredCustomers.map(customer => (
//                 <button
//                   key={customer.customerId}
//                   onClick={() => handleSelectCustomer(customer)}
//                   className="w-full text-left p-3 hover:bg-gray-50 dark:hover:bg-gray-700 border-b border-gray-100 dark:border-gray-600 last:border-b-0"
//                 >
//                   <div className="font-medium text-gray-900 dark:text-gray-100">
//                     {customer.name}
//                   </div>
//                   {customer.phone && (
//                     <div className="text-sm text-gray-600 dark:text-gray-400">
//                       {customer.phone}
//                     </div>
//                   )}
//                 </button>
//               ))
//             )}
//           </motion.div>
//         )}
//       </AnimatePresence>

//       {/* Selected Customer Display */}
//       {selectedCustomer && (
//         <motion.div
//           initial={{ opacity: 0, height: 0 }}
//           animate={{ opacity: 1, height: "auto" }}
//           className="mt-3 p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
//         >
//           <div className="flex items-center justify-between">
//             <div>
//               <div className="font-semibold text-gray-900 dark:text-gray-100 flex items-center gap-2">
//                 <User size={16} />
//                 {selectedCustomer.name}
//               </div>
//               {selectedCustomer.phone && (
//                 <div className="text-sm text-gray-600 dark:text-gray-400 mt-1">
//                   {selectedCustomer.phone}
//                 </div>
//               )}
//               <div className="text-sm text-gray-600 dark:text-gray-400">
//                 Balance: Rs. {(selectedCustomer.balance || 0).toFixed(2)}
//               </div>
//             </div>
//             <button
//               onClick={() => onSelectCustomer(null)}
//               className="text-red-500 hover:text-red-700 text-sm"
//             >
//               Remove
//             </button>
//           </div>
//         </motion.div>
//       )}
//     </div>
//   );
// };