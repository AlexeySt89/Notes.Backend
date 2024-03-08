using AutoMapper;
using Notes.Application.Common.Mapping;
using Notes.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.Application.Notes.Queries.GetNoteDetails
{
    public class NoteDetailsVm : IMapWith<Note>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime EditDate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Note, NoteDetailsVm>()
                .ForMember(noteVM => noteVM.Title,
                    opt => opt.MapFrom(note => note.Title))
                .ForMember(noteVM => noteVM.Details,
                    opt => opt.MapFrom(note => note.Details))
                .ForMember(noteVM => noteVM.Id,
                    opt => opt.MapFrom(note => note.Id))
                .ForMember(noteVM => noteVM.DateCreate,
                    opt => opt.MapFrom(note => note.DateCreate))
                .ForMember(noteVM => noteVM.EditDate,
                    opt => opt.MapFrom(note => note.EditDate));
        }
    }
}
