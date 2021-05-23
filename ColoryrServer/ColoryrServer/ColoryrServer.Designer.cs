using System.Runtime.InteropServices;
using System.ComponentModel;

namespace ColoryrServer
{
    partial class ColoryrServer
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                base.Dispose(disposing);
            }
            
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.ServiceName = "ColoryrServer";
            }
        }

        #endregion
    }
}
