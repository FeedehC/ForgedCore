// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ClientVoiceProviderError.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Blizzard.Telemetry.Wow.Client {

  /// <summary>Holder for reflection information generated from ClientVoiceProviderError.proto</summary>
  public static partial class ClientVoiceProviderErrorReflection {

    #region Descriptor
    /// <summary>File descriptor for ClientVoiceProviderError.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ClientVoiceProviderErrorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ch5DbGllbnRWb2ljZVByb3ZpZGVyRXJyb3IucHJvdG8SHUJsaXp6YXJkLlRl",
            "bGVtZXRyeS5Xb3cuQ2xpZW50Ghp0ZWxlbWV0cnlfZXh0ZW5zaW9ucy5wcm90",
            "bxoiVGVsZW1ldHJ5U2hhcmVkQ2xpZW50SW1wb3J0cy5wcm90bxoadm9pY2Vf",
            "cHJvdmlkZXJfZXJyb3IucHJvdG8i3gEKGENsaWVudFZvaWNlUHJvdmlkZXJF",
            "cnJvchI5CgZjbGllbnQYASABKAsyKS5CbGl6emFyZC5UZWxlbWV0cnkuV293",
            "LkNsaWVudC5DbGllbnRJbmZvEjcKBXdvcmxkGAIgASgLMiguQmxpenphcmQu",
            "VGVsZW1ldHJ5Lldvdy5DbGllbnQuV29ybGRJbmZvEkUKDnByb3ZpZGVyX2Vy",
            "cm9yGAMgASgLMi0uQmxpenphcmQuVGVsZW1ldHJ5LlZvaWNlQ2xpZW50LlBy",
            "b3ZpZGVyRXJyb3I6B8LMJQOgBgE="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Blizzard.Telemetry.TelemetryExtensionsReflection.Descriptor, global::Blizzard.Telemetry.Wow.Client.TelemetrySharedClientImportsReflection.Descriptor, global::Blizzard.Telemetry.VoiceClient.VoiceProviderErrorReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Blizzard.Telemetry.Wow.Client.ClientVoiceProviderError), global::Blizzard.Telemetry.Wow.Client.ClientVoiceProviderError.Parser, new[]{ "Client", "World", "ProviderError" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ClientVoiceProviderError : pb::IMessage<ClientVoiceProviderError>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ClientVoiceProviderError> _parser = new pb::MessageParser<ClientVoiceProviderError>(() => new ClientVoiceProviderError());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ClientVoiceProviderError> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Blizzard.Telemetry.Wow.Client.ClientVoiceProviderErrorReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ClientVoiceProviderError() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ClientVoiceProviderError(ClientVoiceProviderError other) : this() {
      client_ = other.client_ != null ? other.client_.Clone() : null;
      world_ = other.world_ != null ? other.world_.Clone() : null;
      providerError_ = other.providerError_ != null ? other.providerError_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ClientVoiceProviderError Clone() {
      return new ClientVoiceProviderError(this);
    }

    /// <summary>Field number for the "client" field.</summary>
    public const int ClientFieldNumber = 1;
    private global::Blizzard.Telemetry.Wow.Client.ClientInfo client_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Blizzard.Telemetry.Wow.Client.ClientInfo Client {
      get { return client_; }
      set {
        client_ = value;
      }
    }

    /// <summary>Field number for the "world" field.</summary>
    public const int WorldFieldNumber = 2;
    private global::Blizzard.Telemetry.Wow.Client.WorldInfo world_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Blizzard.Telemetry.Wow.Client.WorldInfo World {
      get { return world_; }
      set {
        world_ = value;
      }
    }

    /// <summary>Field number for the "provider_error" field.</summary>
    public const int ProviderErrorFieldNumber = 3;
    private global::Blizzard.Telemetry.VoiceClient.ProviderError providerError_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::Blizzard.Telemetry.VoiceClient.ProviderError ProviderError {
      get { return providerError_; }
      set {
        providerError_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ClientVoiceProviderError);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ClientVoiceProviderError other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Client, other.Client)) return false;
      if (!object.Equals(World, other.World)) return false;
      if (!object.Equals(ProviderError, other.ProviderError)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (client_ != null) hash ^= Client.GetHashCode();
      if (world_ != null) hash ^= World.GetHashCode();
      if (providerError_ != null) hash ^= ProviderError.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (client_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Client);
      }
      if (world_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(World);
      }
      if (providerError_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ProviderError);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (client_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Client);
      }
      if (world_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(World);
      }
      if (providerError_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ProviderError);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (client_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Client);
      }
      if (world_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(World);
      }
      if (providerError_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ProviderError);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ClientVoiceProviderError other) {
      if (other == null) {
        return;
      }
      if (other.client_ != null) {
        if (client_ == null) {
          Client = new global::Blizzard.Telemetry.Wow.Client.ClientInfo();
        }
        Client.MergeFrom(other.Client);
      }
      if (other.world_ != null) {
        if (world_ == null) {
          World = new global::Blizzard.Telemetry.Wow.Client.WorldInfo();
        }
        World.MergeFrom(other.World);
      }
      if (other.providerError_ != null) {
        if (providerError_ == null) {
          ProviderError = new global::Blizzard.Telemetry.VoiceClient.ProviderError();
        }
        ProviderError.MergeFrom(other.ProviderError);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (client_ == null) {
              Client = new global::Blizzard.Telemetry.Wow.Client.ClientInfo();
            }
            input.ReadMessage(Client);
            break;
          }
          case 18: {
            if (world_ == null) {
              World = new global::Blizzard.Telemetry.Wow.Client.WorldInfo();
            }
            input.ReadMessage(World);
            break;
          }
          case 26: {
            if (providerError_ == null) {
              ProviderError = new global::Blizzard.Telemetry.VoiceClient.ProviderError();
            }
            input.ReadMessage(ProviderError);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (client_ == null) {
              Client = new global::Blizzard.Telemetry.Wow.Client.ClientInfo();
            }
            input.ReadMessage(Client);
            break;
          }
          case 18: {
            if (world_ == null) {
              World = new global::Blizzard.Telemetry.Wow.Client.WorldInfo();
            }
            input.ReadMessage(World);
            break;
          }
          case 26: {
            if (providerError_ == null) {
              ProviderError = new global::Blizzard.Telemetry.VoiceClient.ProviderError();
            }
            input.ReadMessage(ProviderError);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
