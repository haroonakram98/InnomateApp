import React, { useState, ChangeEvent } from 'react';
import { useSettingsStore } from '@/store/useSettingsStore.js';
import Button from '@/components/ui/Button.js';

export default function SettingsPage() {
  const { 
    directPrint, 
    useLocalAgent, 
    localAgentUrl, 
    logoDataUrl, 
    toggleDirect, // Changed from toggleDirectPrint
    toggleLocalAgent, 
    setLocalAgentUrl, 
    setLogo 
  } = useSettingsStore();
  
  const [url, setUrl] = useState(localAgentUrl);

  const onLogoChange = (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    
    const reader = new FileReader();
    reader.onload = () => setLogo(reader.result as string);
    reader.readAsDataURL(file);
  };

  const handleSaveUrl = () => {
    setLocalAgentUrl(url);
  };

  return (
    <div className="p-6 space-y-4">
      <h1 className="text-2xl font-semibold">Settings</h1>
      
      <div className="p-4 bg-white rounded shadow space-y-4">
        {/* Direct Print Setting */}
        <label className="flex items-center gap-2 cursor-pointer">
          <input 
            type="checkbox" 
            checked={directPrint} 
            onChange={(e) => toggleDirect(e.target.checked)} // Changed from toggleDirectPrint
          />
          <span className="select-none">Enable Direct WebUSB Print (thermal printers)</span>
        </label>

        {/* Local Agent Setting */}
        <label className="flex items-center gap-2 cursor-pointer">
          <input 
            type="checkbox" 
            checked={useLocalAgent} 
            onChange={(e) => toggleLocalAgent(e.target.checked)} 
          />
          <span className="select-none">Enable Local Print Agent (recommended for Epson L1800)</span>
        </label>

        {/* Local Agent URL */}
        <div className="space-y-2">
          <label className="block font-medium">Local Agent URL</label>
          <input 
            className="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={url} 
            onChange={(e) => setUrl(e.target.value)} 
            placeholder="http://localhost:3800/print"
          />
          <div>
            <Button onClick={handleSaveUrl}>Save URL</Button>
          </div>
        </div>

        {/* Logo Upload */}
        <div className="space-y-2">
          <label className="block font-medium">Upload Logo (for receipts)</label>
          <input 
            type="file" 
            accept="image/*" 
            onChange={onLogoChange}
            className="w-full p-2 border rounded"
          />
          {logoDataUrl && (
            <div className="mt-2">
              <img 
                src={logoDataUrl} 
                alt="Uploaded logo" 
                className="max-w-[200px] max-h-[200px] object-contain border rounded"
              />
            </div>
          )}
        </div>
      </div>
    </div>
  );
}