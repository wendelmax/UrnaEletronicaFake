using System;
using System.Text.RegularExpressions;

namespace UrnaEletronicaFake.UI.Services;

public interface IDataAnonymizationService
{
    string AnonymizeCpf(string cpf);
    string AnonymizeEmail(string email);
    string AnonymizePhone(string phone);
    string AnonymizeName(string name);
    string AnonymizeAddress(string address);
    string AnonymizeDocument(string document);
    bool IsSensitiveData(string data);
}

public class DataAnonymizationService : IDataAnonymizationService
{
    private const string CPF_PATTERN = @"(\d{3})\.?(\d{3})\.?(\d{3})-?(\d{2})";
    private const string EMAIL_PATTERN = @"([a-zA-Z0-9._%+-]+)@([a-zA-Z0-9.-]+\.[a-zA-Z]{2,})";
    private const string PHONE_PATTERN = @"(\(?\d{2}\)?)\s?(\d{4,5})-?(\d{4})";
    private const string DOCUMENT_PATTERN = @"(\d{2})\.?(\d{3})\.?(\d{3})/?(\d{4})-?(\d{2})";
    
    public string AnonymizeCpf(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return cpf;
            
        return Regex.Replace(cpf, CPF_PATTERN, "***.***.***-**");
    }
    
    public string AnonymizeEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return email;
            
        return Regex.Replace(email, EMAIL_PATTERN, "***@***.***");
    }
    
    public string AnonymizePhone(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return phone;
            
        return Regex.Replace(phone, PHONE_PATTERN, "(**) ****-****");
    }
    
    public string AnonymizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
            
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return name;
            
        var anonymizedParts = new string[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length <= 2)
            {
                anonymizedParts[i] = "**";
            }
            else
            {
                anonymizedParts[i] = parts[i][0] + new string('*', parts[i].Length - 2) + parts[i][^1];
            }
        }
        
        return string.Join(" ", anonymizedParts);
    }
    
    public string AnonymizeAddress(string address)
    {
        if (string.IsNullOrEmpty(address))
            return address;
            
        var words = address.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var anonymizedWords = new string[words.Length];
        
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length <= 2)
            {
                anonymizedWords[i] = "**";
            }
            else
            {
                anonymizedWords[i] = words[i][0] + new string('*', words[i].Length - 2) + words[i][^1];
            }
        }
        
        return string.Join(" ", anonymizedWords);
    }
    
    public string AnonymizeDocument(string document)
    {
        if (string.IsNullOrEmpty(document))
            return document;
            
        return Regex.Replace(document, DOCUMENT_PATTERN, "**.***.***/****-**");
    }
    
    public bool IsSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;
            
        return Regex.IsMatch(data, CPF_PATTERN) ||
               Regex.IsMatch(data, EMAIL_PATTERN) ||
               Regex.IsMatch(data, PHONE_PATTERN) ||
               Regex.IsMatch(data, DOCUMENT_PATTERN);
    }
}


