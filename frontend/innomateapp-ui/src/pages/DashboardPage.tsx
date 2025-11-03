// src/pages/DashboardPage.tsx
import MainLayout from "@/components/layout/MainLayout.js";
import SmartOpsDashboard from "@/components/dashboard/SmartOpsDashboard.js";

export default function DashboardPage() {
  return (
    <MainLayout>
      <SmartOpsDashboard />
    </MainLayout>
  );
}
