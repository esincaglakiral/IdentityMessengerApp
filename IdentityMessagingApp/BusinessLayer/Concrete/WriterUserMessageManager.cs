using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class WriterUserMessageManager : IWriterUserMessageService
    {
        IWriterUserMessageDal _writerUserMessage;

        public WriterUserMessageManager(IWriterUserMessageDal writerUserMessage)
        {
            _writerUserMessage = writerUserMessage;
        }

        public List<WriterUserMessage> GetListReceiverMessage(string p)
        {
            return _writerUserMessage.GetByFilter(x => x.Receiver == p).OrderByDescending(x => x.Date).ToList();
        }

        public List<WriterUserMessage> GetListSenderMessage(string p)
        {
            return _writerUserMessage.GetByFilter(x => x.Sender == p).OrderByDescending(x => x.Date).ToList();
        }

        public void TAdd(WriterUserMessage t)
        {
           _writerUserMessage.Insert(t);
        }

        public void TDelete(WriterUserMessage t)
        {
            _writerUserMessage.Delete(t);
        }

        public WriterUserMessage TGetByID(int id)
        {
            return _writerUserMessage.GetByID(id);
        }

        public List<WriterUserMessage> TGetList()
        {
            return _writerUserMessage.GetList();
        }

        public List<WriterUserMessage> TGetListByFilter()
        {
            throw new NotImplementedException();
        }

        public void TUpdate(WriterUserMessage t)
        {
            _writerUserMessage.Update(t);
        }
    }
}
