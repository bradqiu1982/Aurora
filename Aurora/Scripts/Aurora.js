var Aurora = function () {
    var createpage = function () {
        $('body').on('click', '.maineditor-time', function () {
            $('#mainduedatemodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-people', function () {
            $('#relatedmodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.maineditor-proj', function () {
            $('#add-event-modal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.m-event-edit', function () {
            var content = $.trim($(this).parent().parent().next().html());
            var event_no = $(this).parent().prev().html();
            $('#m-event').val(content);
            $('#m-event-no').val(event_no);
        })
        $('body').on('click', '.m-confirm-edit', function () {
            var event_no = $.trim($('#m-event-no').val());
            var content = $.trim($('#m-event').val());
            if (event_no == "") {
                var n_event_no = 1;
                if ($('.event-item').length != 0) {
                    n_event_no = parseInt($('.event-item').last().children().eq(0).children().eq(0).html()) + 1;
                }

                var appendStr = '<div class="event-item">' +
                    '<div class="event-title" >' +
                    '<div class="event-no">' + (n_event_no) +'</div>' +
                    '<div class="event-op">' +
                    '<span class="glyphicon glyphicon-pencil m-event-edit" aria-hidden="true"></span>' +
                    '<span class="glyphicon glyphicon-remove m-event-del" aria-hidden="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<div class="event-content">' + content +
                    '</div>' +
                    '</div>';
                
                    $('.event-all').append(appendStr);

            }
            else {
                $('.event-no').each(function () {
                    if ($.trim($(this).html()) == event_no) {
                        $(this).parent().next().html(content);
                        $('#m-event-no').val('');
                    }
                })
            }

            $('#m-event').val('');
        })

        $('body').on('click', '.m-event-del', function () {
            if (confirm("Really to delete?")) {
                $(this).parent().parent().parent().remove();
            }
        })

        $('body').on('click', '#m-cancel-addevent', function () {
            $('#add-event-modal').modal('hide');
            $('.event-all').empty();
        })

        $('body').on('click', '#m-addevent', function () {
            $('#add-event-modal').modal('hide');
            var eventcontent = '';
            $('.event-content').each(function () {
                eventcontent = eventcontent+'<p>#' + $(this).html() + '</p>';
            });
            if (eventcontent)
            {
                var wholeval = CKEDITOR.instances.JobTopicEditor.getData() + eventcontent;
                 CKEDITOR.instances.JobTopicEditor.setData(wholeval);
            }

        })

        function submitduedate()
        {
            var topicid = $('#topicid').val();
            var duedate = $('#duedate').val();
            var warningclock = '';
            if ($('input[name=warning-clock]:checked').val()) {
                warningclock = $('input[name=warning-clock]:checked').val();
            }

            if(duedate || warningclock)
            {
                $.post('/CoWork/NewTopicDueDate',
                    {
                        topicid:topicid,
                        duedate:duedate,
                        warningclock:warningclock
                    },
                    function(output){});
            }
        }

        function submitpeople()
        {
            var topicid = $('#topicid').val();
            var pps = $('#towhoinput1').val();
            if (pps)
            {
                $.post('/CoWork/NewTopicPeople',
                    {
                        topicid:topicid,
                        pps:pps
                    },
                    function(output){});
            }
        }

        function submitevent()
        {
            var topicid = $('#topicid').val();
            var eventcontents = new Array();
            $('.event-content').each(function () {
                eventcontents.push($(this).html());
            });

            if (eventcontents.length != 0)
            {
                $.post('/CoWork/NewTopicEvent',
                    {
                        topicid: topicid,
                        eventcontents: JSON.stringify(eventcontents)
                    },
                    function (output) {});
            }
        }

        $('body').on('click', '#topiceditorsubmit', function () {
            var subject = $('#subject').val();
            var content = CKEDITOR.instances.JobTopicEditor.getData();
            if(subject && content)
            {
                submitduedate();
                submitpeople();
                submitevent();

                $('#topiceditorform').submit();
            }
            else
            {
                alert("topic subject or topic content should not be empty!")
                return false;
            }
        })

        $('body').on('click', '.logo', function () {
            window.location.href = '/CoWork/Home';
        })
    };

    var mainpage = function () {

        $('body').on('mouseenter', '.search-topic', function () {
            $('#keywords').removeClass('hide');
            setTimeout(
                "if($('#keywords').val == '') $('#keywords').addClass('hide')"
            , 3000);
        });

        $('body').on('mouseleave', '.search-topic', function () {
            if ($('#keywords').val() == '') {
                $('#keywords').addClass('hide');
            }
        })

        $('body').on('click', '.topiclistcls', function () {
            var topicid = $(this).attr('topicid');
            var activenavitem = $('.activenavitem').attr("navid");
            window.location.href = '/CoWork/Home?activenavitem=' + activenavitem + '&topicid='+topicid;
        })

        $('body').on('click', '.home-md-topic', function () {
            var topicid = $('#currenttopicid').val();
            var activenavitem = $('.activenavitem').attr('navid');
            window.location.href = '/CoWork/ModifyTopic?activenavitem=' + activenavitem + '&topicid=' + topicid;
        })

        $('body').on('click', '.home-md-commont', function () {
            var topicid = $('#currenttopicid').val();
            var activenavitem = $('.activenavitem').attr('navid');
            var cid = $(this).attr('cid');
            window.location.href = '/CoWork/ModifyTopic?activenavitem=' + activenavitem + '&topicid=' + topicid+'&commentid='+cid;
        })

        

        $('body').on('click', '.nav-sidebar-icon', function () {
            var ndhide = $('.item-panels').hasClass('hide');

            if ($(this).children().eq(0).hasClass('sidebar-show')) {
                $('.nav-sidebar').addClass('nav-sidebar-narrow');
                $(this).children().eq(0).removeClass('sidebar-show').addClass('sidebar-hide');
                $('.item-show').addClass('hidden');

                if (ndhide) {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-34');
                }
                else {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-9');
                }
            }
            else {
                $('.nav-sidebar').removeClass('nav-sidebar-narrow');
                $(this).children().eq(0).removeClass('sidebar-hide').addClass('sidebar-show');
                $('.item-show').removeClass('hidden');

                if (ndhide) {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-25');
                }
                else {
                    $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7');
                }
            }
        })

        
        $('body').on('click', '.itemlistswitchclose', function () {
            var sthide = $('.nav-sidebar').hasClass('nav-sidebar-narrow');

            $('.item-panels').addClass('hide');
            $('.itemlistswitchopen').removeClass('hide');

            if (sthide) {

                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-34');
            }
            else {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-25');
            }
        })

        $('body').on('click', '.itemlistswitchopen', function () {
            var sthide = $('.nav-sidebar').hasClass('nav-sidebar-narrow');

            $('.item-panels').removeClass('hide');
            $('.itemlistswitchopen').addClass('hide');

            if (sthide) {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7').addClass('item-details-9');
            }
            else {
                $('.item-details-show').removeClass().addClass('item-details-show').addClass('col-sm-7');
            }
        })

        $('body').on('click', '.home-event-item,.SetEventStatusMenu', function () {
            var topicid = $(this).attr('topicid');
            $('#demoparent').empty();
            $('#demoparent').append('<div id="todo-lists-basic-demo"></div>');
            $('#todo-lists-basic-demo').lobiList({
                actions: {
                    load: '/CoWork/InitEventList?topicid=' + topicid,
                    move: '/CoWork/MoveEventList'
                },
                enableTodoRemove: false,
                enableTodoEdit: false,
                controls: []
            });
            $('#eventlistmodal').modal({ backdrop: 'static' });
        })

        $('body').on('click', '.CompleteTopicMenu', function () {
            var topicid = $(this).attr('topicid');
            if (confirm('Do you really want to close this topic ?')) {
                $.post('/CoWork/CompleteTopic'
                    , {
                        topicid: topicid
                    }
                    , function (output) {
                        var activenavitem = $('.activenavitem').attr("navid");
                        window.location.href = '/CoWork/Home?activenavitem=' + activenavitem;
                });
            }
        })
        
        $('body').on('click', '.RemoveTopicMenu', function () {
            var topicid = $(this).attr('topicid');
            if (confirm('Do you really want to remove this topic ?'))
            {
                $.post('/CoWork/RemoveTopic', {
                    topicid: topicid
                }, function (output) {
                    var activenavitem = $('.activenavitem').attr("navid");
                    window.location.href = '/CoWork/Home?activenavitem=' + activenavitem;
                });
            }
        })

        $('body').on('click', '.search-img', function () {
            var keywords = $.trim($('#keywords').val());
            var activenavitem = $('.activenavitem').attr("navid");
            if (keywords)
            {
                window.location.href = '/CoWork/Home?activenavitem=' + activenavitem + '&searchkey=' + keywords;
            }
        })
        
    };

    return {
        createpageinit: function ()
        {
            createpage();
        },
        mainpageinit:function()
        {
            mainpage();
        }
    };
}();