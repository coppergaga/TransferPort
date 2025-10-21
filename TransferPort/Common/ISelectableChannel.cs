using System.Collections;

namespace RsTransferPort
{
    public interface ISelectableChannel
    {
        IEnumerable GetSelectableList();

        void SetChannel(object target);

        void ClearChannel();

        bool HasChannel(object target);

        string GetRowLabel(object item);

        string GetRowTip(object item);
        bool HasRowTip(object item);
        
        
    }

    // public interface ISelectableChannel<TSource> : ISelectableChannel
    // {
    //     IEnumerable<TSource> GetSelectableList();
    //
    //     void SetChannel(TSource target);
    //     
    //     void ClearChannel();
    //     
    //     TSource GetChannel();
    //
    //     string GetRowLabel(TSource item);
    // } 
}