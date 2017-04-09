using System;
using System.Collections.Generic;
using System.Linq;
using Learning.Calculator.EntityWrapper.Properties;
using Learning.Calculator.Models;

namespace Learning.Calculator.EntityWrapper
{
    public static class EntityWrapper
    {
        public static void SaveModel(CalculatorModel model)
        {
            try
            {
                using (var context = new CalculatorDbContext())
                {
                    context.CalculatorModels.Add(model);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, String.Format(Strings.Exception_FailedToSaveObject, typeof(CalculatorModel).Name));
            }
        }

        public static List<CalculatorModel> GetModels()
        {
            try
            {
                using (var context = new CalculatorDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    var res = context.CalculatorModels.Include("Tokens").ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        res[i].Tokens = res[i].Tokens.OrderBy(t => t.Index).ToList();
                    }
                    return res;
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Log(ex, String.Format(Strings.Exception_FailedToGetObjects, typeof(CalculatorModel).Name));
                throw;
            }
        }
    }
}
