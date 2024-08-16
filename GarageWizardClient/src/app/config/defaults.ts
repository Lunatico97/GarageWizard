
export class Defaults
{
    // Regex
    static VehicleStringRegex : string = "^[a-zA-Z][a-zA-Z0-9- ]*$";
    static VehicleIDRegex = /\b[A-Z]{2}\d{3}\b/; // pattern within word boundary (\b) protected by escape sequence
    static SpotIDRegex = /\b[A-Z]{1}[A-Z0-9]{3}\b/ 
    static EmailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    // Messages
    
}