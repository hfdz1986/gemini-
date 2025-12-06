import { GoogleGenAI, Type } from "@google/genai";
import { CustomerFormData, CustomerStatus } from "../types";

const apiKey = process.env.API_KEY;

// Initialize Gemini Client
// Note: We create a new instance per call in complex apps, but a singleton is fine here 
// if the key doesn't change. However, adhering to best practices for robustness.
const getClient = () => {
  if (!apiKey) {
    throw new Error("API Key is missing. Please check your environment configuration.");
  }
  return new GoogleGenAI({ apiKey });
};

export const generateFakeCustomerProfile = async (): Promise<CustomerFormData> => {
  const ai = getClient();
  
  const prompt = "Generate a realistic, fictional customer profile for a business client in China. Include name (Chinese name), email, phone, company name, status, and a short professional note.";

  try {
    const response = await ai.models.generateContent({
      model: 'gemini-2.5-flash',
      contents: prompt,
      config: {
        responseMimeType: "application/json",
        responseSchema: {
          type: Type.OBJECT,
          properties: {
            name: { type: Type.STRING },
            email: { type: Type.STRING },
            phone: { type: Type.STRING },
            company: { type: Type.STRING },
            status: { type: Type.STRING, enum: [CustomerStatus.Active, CustomerStatus.Inactive, CustomerStatus.Lead] },
            notes: { type: Type.STRING },
          },
          required: ["name", "email", "phone", "company", "status", "notes"],
        }
      }
    });

    const text = response.text;
    if (!text) throw new Error("No data returned from AI");

    const data = JSON.parse(text) as CustomerFormData;
    
    // Ensure the enum matches, fallback if AI hallucinates a different string
    let status = CustomerStatus.Lead;
    if (Object.values(CustomerStatus).includes(data.status)) {
        status = data.status;
    }

    return {
        ...data,
        status
    };

  } catch (error) {
    console.error("Gemini AI generation error:", error);
    throw error;
  }
};
