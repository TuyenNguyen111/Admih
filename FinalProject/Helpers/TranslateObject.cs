namespace FinalProject.Helpers
{
    public class TranslateObject
    {
        public static string Translate(string objectName)
        {
            switch (objectName)
            {
                case "person":
                    return "Người";
                case "car":
                    return "Xe hơi";
                case "dog":
                    return "Chó";
                case "cat":
                    return "Mèo";
                case "horse":
                    return "Ngựa";
                case "sheep":
                    return "Cừu";
                case "cow":
                    return "Bò";
                case "bird":
                    return "Chim";
                case "truck":
                    return "Xe tải";
                case "bus":
                    return "Xe buýt";
                case "motorcycle":
                    return "Xe máy";
                case "bicycle":
                    return "Xe đạp";
                case "airplane":
                    return "Máy bay";
                case "train":
                    return "Tàu hỏa";
                case "van":
                    return "Xe van";
                case "boat":
                    return "Thuyền";
                default:
                    return objectName; 
            }
        }
    }
}
