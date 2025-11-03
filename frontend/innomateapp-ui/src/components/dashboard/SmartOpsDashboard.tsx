import React from "react";
import { Card, CardContent } from "@/components/ui/Card.js";
import Button from "@/components/ui/Button.js";
import {
  TrendingUp,
  Users,
  DollarSign,
  Database,
  RefreshCw,
} from "lucide-react";

const SmartOpsDashboard: React.FC = () => {
  const stats = [
    {
      title: "Active Customers",
      value: "1,245",
      icon: (
        <Users
          className="text-blue-600 dark:text-blue-400 transition-colors duration-300"
          size={24}
        />
      ),
    },
    {
      title: "Monthly Revenue",
      value: "PKR 3.2M",
      icon: (
        <DollarSign
          className="text-green-600 dark:text-green-400 transition-colors duration-300"
          size={24}
        />
      ),
    },
    {
      title: "Growth Rate",
      value: "+26%",
      icon: (
        <TrendingUp
          className="text-purple-600 dark:text-purple-400 transition-colors duration-300"
          size={24}
        />
      ),
    },
    {
      title: "Database Size",
      value: "512 MB",
      icon: (
        <Database
          className="text-orange-600 dark:text-orange-400 transition-colors duration-300"
          size={24}
        />
      ),
    },
  ];

  return (
    <div className="space-y-6 transition-colors duration-300">
      <div className="flex flex-wrap justify-between items-center gap-3">
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100 transition-colors duration-300">
          Overview
        </h1>
        <div className="flex gap-2 flex-wrap">
          <Button variant="outline">
            <RefreshCw size={16} className="inline-block mr-1" /> Refresh
          </Button>
          <Button>Add Record</Button>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4">
        {stats.map((s) => (
          <Card
            key={s.title}
            className="transition-colors duration-300"
          >
            <CardContent className="flex justify-between items-center p-5">
              <div>
                <p className="text-sm text-gray-500 dark:text-gray-400 transition-colors duration-300">
                  {s.title}
                </p>
                <h3 className="text-xl font-semibold text-gray-900 dark:text-gray-100 transition-colors duration-300">
                  {s.value}
                </h3>
              </div>
              {s.icon}
            </CardContent>
          </Card>
        ))}
      </div>

      <Card className="transition-colors duration-300">
        <CardContent className="p-6 text-center text-gray-500 dark:text-gray-400 transition-colors duration-300">
          Analytics & reports coming soon...
        </CardContent>
      </Card>
    </div>
  );
};

export default SmartOpsDashboard;
