﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using SM64_Diagnostic.Managers;
using SM64_Diagnostic.Utilities;
using SM64_Diagnostic.Structs;
using SM64_Diagnostic.Controls;
using SM64_Diagnostic.Extensions;
using System.Drawing.Drawing2D;
using static SM64_Diagnostic.Managers.FileManager;
using SM64_Diagnostic.Structs.Configurations;

namespace SM64_Diagnostic
{
    public class FileHatLocationPictureBox : FilePictureBox
    {
        private HatLocation _definingHatLocation;
        private HatLocation? _currentHatLocation;
        private Image _onImage;
        private Image _offImage;

        public FileHatLocationPictureBox()
        {
        }

        public void Initialize(HatLocation definingHatLocation, Image onImage, Image offImage)
        {
            _definingHatLocation = definingHatLocation;
            _onImage = onImage;
            _offImage = offImage;
            base.Initialize(0, 0);
        }

        private HatLocation? GetCurrentHatLocation()
        {
            ushort hatLocationCourse = Config.Stream.GetUInt16(FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationCourseOffset);
            byte hatLocationMode = (byte)(Config.Stream.GetByte(FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationModeOffset) & FileConfig.HatLocationModeMask);

            return hatLocationMode == FileConfig.HatLocationMarioMask ? HatLocation.Mario :
                   hatLocationMode == FileConfig.HatLocationKleptoMask ? HatLocation.SSLKlepto :
                   hatLocationMode == FileConfig.HatLocationSnowmanMask ? HatLocation.SLSnowman :
                   hatLocationMode == FileConfig.HatLocationUkikiMask ? HatLocation.TTMUkiki :
                   hatLocationMode == FileConfig.HatLocationGroundMask ?
                       (hatLocationCourse == FileConfig.HatLocationCourseSSLValue ? HatLocation.SSLGround :
                        hatLocationCourse == FileConfig.HatLocationCourseSLValue ? HatLocation.SLGround :
                        hatLocationCourse == FileConfig.HatLocationCourseTTMValue ? HatLocation.TTMGround :
                        (HatLocation?)null) :
                   null;
        }

        private Image GetImageForValue(HatLocation? hatLocation)
        {
            if (_definingHatLocation == hatLocation)
                return _onImage;
            else
                return _offImage;
        }


        protected override void ClickAction(object sender, EventArgs e)
        {
            switch (_definingHatLocation)
            {
                case HatLocation.Mario:
                    SetHatMode(FileConfig.HatLocationMarioMask);
                    break;

                case HatLocation.SSLKlepto:
                    SetHatMode(FileConfig.HatLocationKleptoMask);
                    break;

                case HatLocation.SSLGround:
                    SetHatMode(FileConfig.HatLocationGroundMask);
                    Config.Stream.SetValue(FileConfig.HatLocationCourseSSLValue, FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationCourseOffset);
                    break;

                case HatLocation.SLSnowman:
                    SetHatMode(FileConfig.HatLocationSnowmanMask);
                    break;

                case HatLocation.SLGround:
                    SetHatMode(FileConfig.HatLocationGroundMask);
                    Config.Stream.SetValue(FileConfig.HatLocationCourseSLValue, FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationCourseOffset);
                    break;

                case HatLocation.TTMUkiki:
                    SetHatMode(FileConfig.HatLocationUkikiMask);
                    break;

                case HatLocation.TTMGround:
                    SetHatMode(FileConfig.HatLocationGroundMask);
                    Config.Stream.SetValue(FileConfig.HatLocationCourseTTMValue, FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationCourseOffset);
                    break;
            }
        }

        private void SetHatMode(byte hatModeByte)
        {
            byte oldByte = Config.Stream.GetByte(FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationModeOffset);
            byte newByte = MoreMath.ApplyValueToMaskedByte(oldByte, FileConfig.HatLocationModeMask, hatModeByte);
            Config.Stream.SetValue(newByte, FileManager.Instance.CurrentFileAddress + FileConfig.HatLocationModeOffset);
        }

        public override void UpdateImage(bool force = false)
        {
            HatLocation? currentHatLocation = GetCurrentHatLocation();
            if (_currentHatLocation != currentHatLocation || force)
            {
                this.Image = GetImageForValue(currentHatLocation);
                _currentHatLocation = currentHatLocation;
                Invalidate();
            }
        }
    }
}
